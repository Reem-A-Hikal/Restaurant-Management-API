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
    }
}
