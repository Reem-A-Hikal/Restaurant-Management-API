using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<User?> ValidateUserCredentialsAsync(string email, string password);

        /// <summary>Generates a new refresh token, persists its hash, returns the raw value.</summary>
        Task<string> GenerateRefreshTokenAsync(string userId);

        /// <summary>Looks up a refresh token by its raw value, regardless of status.</summary>
        Task<RefreshToken?> FindRefreshTokenAsync(string rawToken);

        /// <summary>Revokes the given token and issues a new one in its place. Returns the new raw token.</summary>
        Task<string> RotateRefreshTokenAsync(RefreshToken currentToken);

        Task RevokeRefreshTokenAsync(RefreshToken token);
        Task RevokeAllUserRefreshTokensAsync(string userId);
    }
}
