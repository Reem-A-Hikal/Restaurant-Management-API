using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;

namespace Rest.Infrastructure.Implementations.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<User> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        //private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public UserService(UserManager<User> userManager, IUserRepository userRepository, IMapper mapper)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _mapper = mapper;

        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users).ToList();
            foreach (var userDto in userDtos)
            {
                var user = await _userManager.FindByIdAsync(userDto.Id);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDto.Roles = roles.ToList();
                }
            }
            return userDtos;
        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = [.. (await _userManager.GetRolesAsync(user))];
            return userDto;
        }

        

        public async Task UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId) ?? throw new Exception("User not found");

            user.FullName = dto.FullName;
            user.PhoneNumber = dto.PhoneNumber;
            user.ProfileImageUrl = dto.ProfileImageUrl;

            switch (user)
            {
                case Chef chef:
                    chef.Specialization = dto.Specialization;
                    break;

                case DeliveryPerson deliveryPerson:
                    deliveryPerson.VehicleNumber = dto.VehicleNumber;
                    break;

                default:
                    break;
            }
            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
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
