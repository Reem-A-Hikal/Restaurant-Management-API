using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Interfaces.IServices;
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
            
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
                throw new ApplicationException("User with this email already exists");
            
            var user = _mapper.Map<User>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                throw new ApplicationException(
                    string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
            await _userManager.AddToRoleAsync(user, "Customer");
        }
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            
            var user = await _authService.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password);
            if (user == null)
                throw new ApplicationException("Invalid email or password");

            var token = await _authService.GenerateJwtTokenAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            return new LoginResponse()
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Token = token,
                Role = userRoles[0],

            };
            
        }
    }
}
