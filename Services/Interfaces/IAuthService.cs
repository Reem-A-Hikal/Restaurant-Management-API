using Rest.API.Dtos.AccountDtos;
using Rest.API.Models;

namespace Rest.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> GenerateJwtTokenAsync(User user);
        Task<User> ValidateUserCredentialsAsync(string email, string password);
        Task<bool> UserExistsAsync(string email);
    }
}
