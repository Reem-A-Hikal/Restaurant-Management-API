using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Rest.Infrastructure.Implementations.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(UserManager<User> userManager,
            IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Name, user.FullName ?? user.UserName!),
                new (ClaimTypes.Email, user.Email !),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["DurationInMinutes"])),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<User?> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                if (user.Status != UserStatus.Active)
                    throw new ValidationException("Account is not active");
                return user;
            }
            
            return null;
        }

        #region Refresh Tokens
        public async Task<string> GenerateRefreshTokenAsync(string userId)
        {
            var rawToken = GenerateSecureRawToken();
            var tokenHash = Hash(rawToken);
            var daysValid = GetRefreshTokenDurationInDays();

            var entity = RefreshToken.Create(userId, tokenHash, daysValid);

            await _refreshTokenRepository.AddAsync(entity);
            await _refreshTokenRepository.SaveChangesAsync();

            return rawToken;
        }

        public async Task<RefreshToken?> FindRefreshTokenAsync(string rawToken)
        {
            var tokenHash = Hash(rawToken);
            return await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
        }

        public async Task<string> RotateRefreshTokenAsync(RefreshToken currentToken)
        {
            var newRawToken = GenerateSecureRawToken();
            var newTokenHash = Hash(newRawToken);
            var daysValid = GetRefreshTokenDurationInDays();

            currentToken.Revoke(newTokenHash);

            var newEntity = RefreshToken.Create(currentToken.UserId, newTokenHash, daysValid);
            await _refreshTokenRepository.AddAsync(newEntity);

            await _refreshTokenRepository.SaveChangesAsync();

            return newRawToken;
        }

        public async Task RevokeRefreshTokenAsync(RefreshToken token)
        {
            if (!token.IsRevoked)
                token.Revoke();

            await _refreshTokenRepository.SaveChangesAsync();
        }

        public async Task RevokeAllUserRefreshTokensAsync(string userId)
        {
            await _refreshTokenRepository.RevokeAllActiveForUserAsync(userId);
            await _refreshTokenRepository.SaveChangesAsync();
        }

        private int GetRefreshTokenDurationInDays()
        {
            var configValue = _configuration["JwtSettings:RefreshTokenDurationInDays"];
            return int.TryParse(configValue, out var days) ? days : 7;
        }

        private static string GenerateSecureRawToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }

        private static string Hash(string rawToken)
        {
            var bytes = Encoding.UTF8.GetBytes(rawToken);
            var hashBytes = SHA256.HashData(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        #endregion
    }
}