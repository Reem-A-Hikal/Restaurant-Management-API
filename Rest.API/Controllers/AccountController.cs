using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.Interfaces.IServices;
using Swashbuckle.AspNetCore.Annotations;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for handling user authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the AccountController
        /// </summary>
        /// <param name="accountService">Service for account related operations</param>
        public AccountController( IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        [HttpPost("register")]
        [EnableRateLimiting("AuthPolicy")]
        [SwaggerOperation(Summary = "Register a new user", Description = "Register a new user with email, password and Phone Number")]
        [SwaggerResponse(StatusCodes.Status200OK, "User registered successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
            
            await _accountService.RegisterAsync(registerDto);
            return SuccessResponse<string>(null, "Registration successful");
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>Authentication token and user details</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/account/login
        ///     {
        ///         "email": "user@example.com",
        ///         "password": "Password123!"
        ///     }
        /// </remarks>
        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        [SwaggerOperation(
            Summary = "Login a user",
            Description = "Login with email and password, returns JWT token")]
        [SwaggerResponse(StatusCodes.Status200OK, "User logged in successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid email or password")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var response = await _accountService.LoginAsync(loginDto);
            return SuccessResponse(response, "Login successful");
        }

        /// <summary>
        /// Exchanges a valid refresh token for a new access token + refresh token pair.
        /// </summary>
        [HttpPost("refresh-token")]
        [EnableRateLimiting("AuthPolicy")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Refresh access token",
            Description = "Exchanges a valid, non-expired, non-revoked refresh token for a new token pair.")]
        [SwaggerResponse(StatusCodes.Status200OK, "New token pair issued")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Missing refresh token")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Invalid, expired, or revoked refresh token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var response = await _accountService.RefreshTokenAsync(dto.RefreshToken);
            return SuccessResponse(response, "Token refreshed successfully");
        }

        /// <summary>
        /// Revokes the given refresh token, ending that session.
        /// </summary>
        [HttpPost("logout")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Logout", Description = "Revokes the given refresh token.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Logged out successfully")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            await _accountService.LogoutAsync(dto.RefreshToken);
            return SuccessResponse<string>(null, "Logged out successfully");
        }
    }
}
