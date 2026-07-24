using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
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
            var refreshToken = await _authService.GenerateRefreshTokenAsync(user.Id);
            var userRoles = await _userManager.GetRolesAsync(user);

            user.RecordLogin();
            await _userManager.UpdateAsync(user);

            return new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                RefreshToken = refreshToken,
                Role = userRoles[0],
            };

        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            var existingToken = await _authService.FindRefreshTokenAsync(refreshToken)
                ?? throw new ForbiddenException("Invalid refresh token.");

            if (existingToken.IsRevoked)
            {
                await _authService.RevokeAllUserRefreshTokensAsync(existingToken.UserId);
                throw new ForbiddenException(
                    "This refresh token has already been used." +
                    " All sessions have been revoked for security — please sign in again.");
            }

            if (existingToken.IsExpired)
                throw new ForbiddenException("Refresh token has expired. Please sign in again.");

            var user = await _userManager.FindByIdAsync(existingToken.UserId)
                ?? throw new NotFoundException("User", existingToken.UserId);

            if(user.Status != UserStatus.Active)
                throw new ForbiddenException("Account is not active.");

            var newAccessToken = await _authService.GenerateJwtTokenAsync(user);
            var newRefreshToken = await _authService.RotateRefreshTokenAsync(existingToken);
            var userRoles = await _userManager.GetRolesAsync(user);

            return new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                Role = userRoles.FirstOrDefault() ?? string.Empty
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await _authService.FindRefreshTokenAsync(refreshToken);

            if (token == null || token.IsRevoked)
                return;

            await _authService.RevokeRefreshTokenAsync(token);
        }
    }
}
