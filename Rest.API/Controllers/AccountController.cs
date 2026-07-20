using Microsoft.AspNetCore.Mvc;
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
    }
}
