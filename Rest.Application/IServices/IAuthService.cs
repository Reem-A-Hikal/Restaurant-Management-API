using Rest.Domain.Entities;

namespace Rest.Application.IServices
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<User> ValidateUserCredentialsAsync(string email, string password);
        Task<bool> UserExistsAsync(string email);
    }
}
