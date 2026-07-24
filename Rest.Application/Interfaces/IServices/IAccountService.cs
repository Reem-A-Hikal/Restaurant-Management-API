using Rest.Application.Dtos.AccountDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Interfaces.IServices
{
    public interface IAccountService
    {
        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="registerDto">The registration details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RegisterAsync(RegisterDto registerDto);
        /// <summary>
        /// Logs in a user with the provided credentials.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>A task representing the asynchronous operation, containing the authentication token.</returns>
        Task<LoginResponse> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Exchanges a valid refresh token for a new access + refresh token pair.
        /// </summary>
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);

        /// <summary>Revokes the given refresh token.</summary>
        Task LogoutAsync(string refreshToken);
    }
}
