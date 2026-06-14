using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
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

        public UserService(
            UserManager<User> userManager,
            IRoleStrategyResolver roleResolver,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleResolver = roleResolver;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);

            var rolesDict = await _userRepository.GetUsersRolesDictAsync(userDtos.Select(u => u.Id));

            foreach (var userDto in userDtos)
            {
                userDto.Role = rolesDict.TryGetValue(userDto.Id, out var role) 
                    ? role : string.Empty;
                await _userRepository.BulkEnrichUsersAsync(userDtos);
            }

            return userDtos;
        }

        public async Task<PaginatedList<UserDto>> GetPaginatedUsersWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedRole)
        {
            
            var query = _userRepository.GetFilteredUsers(searchTerm, selectedRole);
                
            var paginatedUsers = await PaginatedList<User>.CreateAsync(query, pageIndex, pageSize);
            var mappedItems = _mapper.Map<List<UserDto>>(paginatedUsers.Items);

            var userIds = mappedItems.Select(u => u.Id).ToList();
            var rolesDict = await _userRepository.GetUsersRolesDictAsync(userIds);

            foreach (var item in mappedItems)
            {
                item.Role = rolesDict.TryGetValue(item.Id, out var role) 
                    ? role : string.Empty;
                await _userRepository.BulkEnrichUsersAsync(mappedItems);
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
            await _userRepository.BulkEnrichUsersAsync(new List<UserDto> { userDto });

            return userDto;
        }

        public async Task<string> AddUser(CreateUserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
                throw new ValidationException("User with this email already exists");

            var validRoles = new[] { "Admin", "Chef", "DeliveryPerson", "Customer" };
            if(!validRoles.Contains(userDto.UserRole))
                throw new ValidationException($"Invalid role '{userDto.UserRole}'" + $"Valid roles: {string.Join(",", validRoles)}");

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

            if(dto.Status.HasValue)
            {
                if (dto.Status == UserStatus.Deleted)
                    throw new BusinessException("Cannot set status to Deleted. Use the Delete Button instead.");
                user.Status = dto.Status.Value;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new ValidationException(result.Errors.Select(e => e.Description));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();
            if (primaryRole == null) return;

            try
            {
                var strategy = _roleResolver.Resolve(primaryRole);
                await strategy.UpdateRoleDataAsync(userId, dto);
            }
            catch (BusinessException) { }
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

            var roles = await _userManager.GetRolesAsync(user);
            if(roles.Contains("Admin"))
            {
                var adminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
                if(adminCount <= 1)
                    throw new BusinessException("Cannot delete the last admin user");
            }

            user.Status = UserStatus.Deleted;
            user.Email = $"deleted_{user.Id}@deleted.com";
            user.FullName = $"deleted_{user.Id}";
            user.PhoneNumber = null;
            user.ProfileImageUrl = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));
        }
    }
}
