using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
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

        public async Task<string> AddUser(CreateUserDto userDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                throw new ApplicationException("User with this email already exists");
            }
      
            User user = userDto.UserRole switch
            {
                UserRole.Chef => _mapper.Map<Chef>(userDto),
                UserRole.DeliveryPerson => _mapper.Map<DeliveryPerson>(userDto),
                _ => _mapper.Map<User>(userDto)
            };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (!result.Succeeded)
                throw new ApplicationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, userDto.UserRole.ToString());

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

            //switch (user)
            //{
            //    case Chef chef:
            //        if (roles.Contains("Chef") && !string.IsNullOrEmpty(dto.Specialization))
            //            chef.Specialization = dto.Specialization;
            //        break;

            //    case DeliveryPerson deliveryPerson:
            //        if (roles.Contains("DeliveryPerson") && !string.IsNullOrEmpty(dto.VehicleNumber))
            //            deliveryPerson.VehicleNumber = dto.VehicleNumber;

            //        if (roles.Contains("DeliveryPerson") && dto.IsAvailable.HasValue)
            //            deliveryPerson.IsAvailable = dto.IsAvailable.Value;
            //        break;

            //    default:
            //        break;
            //}
            if ( roles.Contains("Chef"))
            {
                var chef = await _userRepository.GetChefByIdAsync(userId);
                if (chef != null && !string.IsNullOrEmpty(dto.Specialization))
                    chef.Specialization = dto.Specialization;
            }
            else if ( roles.Contains("DeliveryPerson"))
            {
                var deliveryPerson = await _userRepository.GetDeliveryPersonByIdAsync(userId);
                if (deliveryPerson != null)
                {
                    if (!string.IsNullOrEmpty(dto.VehicleNumber))
                        deliveryPerson.VehicleNumber = dto.VehicleNumber;

                    if (dto.IsAvailable.HasValue)
                        deliveryPerson.IsAvailable = dto.IsAvailable.Value;
                }
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
