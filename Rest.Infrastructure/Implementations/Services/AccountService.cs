using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Application.IServices;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountService(UserManager<User> userManager, IAuthService authService, IMapper mapper)
        {
            _userManager = userManager;
            _authService = authService;
            _mapper = mapper;
        }
        public async Task RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    throw new ApplicationException("User with this email already exists");
                }
                var user = _mapper.Map<User>(registerDto);
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    return;
                }
                else
                {
                    throw new ApplicationException(
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred while registering the user: {ex.Message}");
            }
        }
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _authService.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password);
                if (user == null)
                {
                    throw new ApplicationException("Invalid email or password");
                }

                var token = await _authService.GenerateJwtTokenAsync(user);
                var userRoles = await _userManager.GetRolesAsync(user);
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = [.. userRoles];

                return new LoginResponse()
                {
                    Email = loginDto.Email,
                    FullName = userDto.FullName,
                    Token = token,
                    Roles = userDto.Roles
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"An error occurred during login: {ex.Message}");
            }
        }
    }
}
