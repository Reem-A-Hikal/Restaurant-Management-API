using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IRoleStrategyResolver _roleResolver;
        private readonly IMapper _mapper;
        private readonly RestDbContext _context;

        public UserService(UserManager<User> userManager, IRoleStrategyResolver roleResolver, IUserRepository userRepository, IMapper mapper, RestDbContext context)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleResolver = roleResolver;
            _mapper = mapper;
            _context = context;

        }

        private async Task<Dictionary<string, string>> GetUsersRolesDictAsync(IEnumerable<string> userIds)
        {
            var userRoles = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where userIds.Contains(ur.UserId)
                select new { ur.UserId, r.Name }
            ).ToListAsync();

            return userRoles.GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Name).FirstOrDefault() ?? string.Empty
                );
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);

            var rolesDict = await GetUsersRolesDictAsync(userDtos.Select(u => u.Id));

            foreach (var userDto in userDtos)
            {
                userDto.Role = rolesDict.TryGetValue(userDto.Id, out var role) 
                    ? role : string.Empty;
                await EnrichWithRoleDataAsync(userDto);
            }

            return userDtos;
        }

        public async Task<PaginatedList<UserDto>> GetPaginatedUsersWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedRole)
        {
            
            var query = _userRepository.GetFilteredUsers(searchTerm, selectedRole);
                
            var paginatedUsers = await PaginatedList<User>.CreateAsync(query, pageIndex, pageSize);
            var mappedItems = _mapper.Map<List<UserDto>>(paginatedUsers.Items);

            var userIds = mappedItems.Select(u => u.Id).ToList();
            var rolesDict = await GetUsersRolesDictAsync(userIds);

            foreach (var item in mappedItems)
            {
                item.Role = rolesDict.TryGetValue(item.Id, out var role) 
                    ? role : string.Empty;
                await EnrichWithRoleDataAsync(item);
            }
            return new PaginatedList<UserDto>(mappedItems, paginatedUsers.TotalItems, pageIndex, pageSize);
            
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var userDto = _mapper.Map<UserDto>(user);
            var roles = await _userManager.GetRolesAsync(user);

            userDto.Role = roles.FirstOrDefault() ?? string.Empty;
            await EnrichWithRoleDataAsync(userDto);

            return userDto;
        }

        public async Task<string> AddUser(CreateUserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new ValidationException("User with this email already exists");

            var strategy = _roleResolver.Resolve(userDto.UserRole);
            var user = strategy.CreateUserEntity(userDto);

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            var roleResult = await _userManager.AddToRoleAsync(user, userDto.UserRole);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new ValidationException(roleResult.Errors.Select(e => e.Description));
            }
            return user.Id;
        }

        public async Task AdminUpdateUserAsync(string userId, AdminUpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            user.Status = dto.Status;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();

            if (!string.IsNullOrEmpty(primaryRole))
            {
                try
                {
                    var strategy = _roleResolver.Resolve(primaryRole);
                    var roleDataDto = new UpdateProfileDto
                    {
                        Specialization = dto.Specialization,
                        VehicleNumber = dto.VehicleNumber,
                        IsAvailable = dto.IsAvailable
                    };
                    await strategy.UpdateRoleDataAsync(userId, roleDataDto);
                }
                catch (KeyNotFoundException) {}
            }
        }

        public async Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId) 
                ?? throw new NotFoundException("User", userId);

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl))
                user.ProfileImageUrl = dto.ProfileImageUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ValidationException( result.Errors.Select(e => e.Description));
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new ValidationException(result.Errors.Select(e => e.Description));
            }
        }

        private async Task EnrichWithRoleDataAsync(UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Role)) return;
            try
            {
                var strategy = _roleResolver.Resolve(userDto.Role);
                await strategy.EnrichDtoAsync(userDto);
            }
            catch (KeyNotFoundException) { }
        }
    }
}
