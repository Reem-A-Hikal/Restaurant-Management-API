using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthService _authService;
        private readonly UserCreationHelper _creationHelper;

        public AccountService(
            UserManager<User> userManager,
            IAuthService authService,
            UserCreationHelper creationHelper)
        {
            _userManager = userManager;
            _authService = authService;
            _creationHelper = creationHelper;
        }
        public async Task RegisterAsync(RegisterDto registerDto)
        {
            var user = User.Create(
                email: registerDto.Email,
                userName: registerDto.Email,
                fullName: registerDto.FullName,
                phoneNumber: registerDto.PhoneNumber,
                profileImageUrl: null);

            await _creationHelper.CreateAndAssignRoleAsync(user, registerDto.Password, "Customer");
        }
        public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
        {
            
            var user = await _authService.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password) 
                ?? throw new ValidationException("Invalid email or password");

            var token = await _authService.GenerateJwtTokenAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            user.RecordLogin();
            await _userManager.UpdateAsync(user);

            return new LoginResponse
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
