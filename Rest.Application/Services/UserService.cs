using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Constants;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IRoleStrategyResolver _roleResolver;
        private readonly UserCreationHelper _creationHelper;
        private readonly IMapper _mapper;
        private readonly IImageUploadService _imageUploadService;

        public UserService(
            UserManager<User> userManager,
            IRoleStrategyResolver roleResolver,
            IUserRepository userRepository,
            UserCreationHelper userCreationHelper,
            IMapper mapper,
            IImageUploadService imageUploadService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _roleResolver = roleResolver;
            _creationHelper = userCreationHelper;
            _mapper = mapper;
            _imageUploadService = imageUploadService;
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
            }
            await _userRepository.BulkEnrichUsersAsync(userDtos);

            return userDtos;
        }

        public async Task<PaginatedList<UserDto>> GetPaginatedUsersWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedRole)
        {

            var paginated = await _userRepository.GetPaginatedAsync(
                pageIndex, pageSize, searchTerm, selectedRole);

            var userIds = paginated.Items.Select(u => u.Id).ToList();
            var rolesDict = await _userRepository.GetUsersRolesDictAsync(userIds);

            var dtos = _mapper.Map<List<UserDto>>(paginated.Items);


            foreach (var dto in dtos)
            {
                dto.Role = rolesDict.TryGetValue(dto.Id, out var role) 
                    ? role : string.Empty;
            }

            await _userRepository.BulkEnrichUsersAsync(dtos);
            return new PaginatedList<UserDto>(dtos, paginated.TotalItems, pageIndex, pageSize);
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
            var validRoles = new[] { "Admin", "Chef", "DeliveryPerson", "Customer" };
            if(!validRoles.Contains(userDto.UserRole))
                throw new ValidationException($"Invalid role '{userDto.UserRole}'" + $"Valid roles: {string.Join(",", validRoles)}");

            var strategy = _roleResolver.Resolve(userDto.UserRole);
            var user = strategy.CreateUserEntity(userDto);

            await _creationHelper.CreateAndAssignRoleAsync(user, userDto.Password, userDto.UserRole);
            return user.Id;
        }

        public async Task AdminUpdateUserAsync(string userId, AdminUpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            if(dto.Status.HasValue)
            {
                user.ChangeStatus(dto.Status.Value);

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    throw new ValidationException(result.Errors.Select(e => e.Description));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault();
            if (primaryRole == null) return;

            var strategy = _roleResolver.Resolve(primaryRole);
            await strategy.UpdateRoleDataAsync(userId, dto);
            
        }

        public async Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var oldImageUrl = user.ProfileImageUrl;

            user.UpdateProfile(dto.FullName, dto.PhoneNumber, dto.ProfileImageUrl);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl)
            && dto.ProfileImageUrl != oldImageUrl
            && !string.IsNullOrWhiteSpace(oldImageUrl))
            {
                _imageUploadService.Delete(oldImageUrl);
            }
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new NotFoundException("User", userId);

            var roles = await _userManager.GetRolesAsync(user);
            if(roles.Contains(AppRoles.Admin))
            {
                var adminCount = (await _userManager.GetUsersInRoleAsync(AppRoles.Admin)).Count;
                if(adminCount <= 1)
                    throw new BusinessException("Cannot delete the last admin user");
            }

            var imageToDelete = user.ProfileImageUrl;

            user.MarkAsDeleted();

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ValidationException(result.Errors.Select(e => e.Description));

            _imageUploadService.Delete(imageToDelete);
        }
    }
}
