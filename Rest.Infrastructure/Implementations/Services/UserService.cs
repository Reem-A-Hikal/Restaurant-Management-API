using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.IServices;
using Rest.Application.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;

namespace Rest.Infrastructure.Implementations.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IRoleStrategyResolver _roleResolver;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, IRoleStrategyResolver roleResolver, IUserRepository userRepository, IMapper mapper)
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
            foreach (var userDto in userDtos)
            {
                var user = await _userManager.FindByIdAsync(userDto.Id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDto.Roles = [.. roles];

                    foreach(var role in roles)
                    {
                        var strategy = _roleResolver.Resolve(role);
                        await strategy.EnrichDtoAsync(userDto);
                    }
                }
            }
            return userDtos;
        }

        public async Task<PaginatedList<UserDto>> GetPaginatedUsersWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedRole)
        {
            try
            {
                var query = _userRepository.GetFilteredUsers(searchTerm, selectedRole);
                
                var paginatedUsers = await PaginatedList<User>.CreateAsync(query, pageIndex, pageSize);
                var mappedItems = _mapper.Map<List<UserDto>>(paginatedUsers.Items);
                foreach (var item in mappedItems)
                {
                    var user = await _userManager.FindByIdAsync(item.Id);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        item.Roles = [.. roles];

                        foreach(var role in roles)
                        {
                            var strategy = _roleResolver.Resolve(role);
                            await strategy.EnrichDtoAsync(item);
                        }
                    }
                }
                return new PaginatedList<UserDto>(mappedItems, paginatedUsers.TotalItems, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not retrieve paginated Users records.", ex);
            }
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            var userDto = _mapper.Map<UserDto>(user);

            userDto.Roles = [.. (await _userManager.GetRolesAsync(user))];
            return userDto;
        }

        public async Task<string> AddUser(CreateUserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new ApplicationException("User with this email already exists");
            }

            var strategy = _roleResolver.Resolve(userDto.UserRole);
            var user = strategy.CreateUserEntity(userDto);

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
                throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, userDto.UserRole);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                throw new ApplicationException(string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
            return user.Id;
        }

        public async Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new Exception("User not found");
            var roles = await _userManager.GetRolesAsync(user);

            user.FullName = dto.FullName ?? user.FullName;
            user.PhoneNumber = dto.PhoneNumber ?? user.PhoneNumber;
            user.ProfileImageUrl = dto.ProfileImageUrl ?? user.ProfileImageUrl;
            foreach (var role in roles)
            {
                var strategy = _roleResolver.Resolve(role);
                await strategy.UpdateRoleDataAsync(userId, dto);
            }

            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
