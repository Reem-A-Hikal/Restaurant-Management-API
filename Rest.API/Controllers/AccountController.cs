using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.AccountDtos;
using Rest.Application.IServices;
using Swashbuckle.AspNetCore.Annotations;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for handling user authentication
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the AccountController
        /// </summary>
        /// <param name="accountService">Service for account related operations</param>
        public AccountController( IAccountService accountService )
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
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _accountService.RegisterAsync(registerDto);
                return Ok(new { message = "Registration successful" });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred during registration: {ex.Message}" });
            }
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
        [SwaggerOperation(Summary = "Login a user", Description = "Login a user with email and password")]
        [SwaggerResponse(StatusCodes.Status200OK, "User logged in successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid email or password")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var response = await _accountService.LoginAsync(loginDto);
                if (response == null)
                    return BadRequest("Invalid email or password");

                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { Message = "An error occurred while logging in", Details = ex.Message });
            }
        }
        ///// <summary>
        ///// Retrieves details for a specific user
        ///// </summary>
        ///// <param name="userId">The ID of the user to retrieve</param>
        ///// <returns>User details including roles</returns>
        ///// <remarks>
        ///// Users can only access their own profile unless they are Admins
        ///// </remarks>
        //[HttpGet("getUser/{userId}")]
        //[Authorize]
        //[SwaggerOperation(Summary = "Get user details", Description = "Get user details by user ID")]
        //[SwaggerResponse(StatusCodes.Status200OK, "User details retrieved successfully")]
        //[SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        //[SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user")]
        //public async Task<IActionResult> GetUser(string userId)
        //{
        //    try
        //    {
        //        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        if (currentUserId != userId && !User.IsInRole("Admin"))
        //        {
        //            //return Unauthorized();
        //            return StatusCode(403, new
        //            {
        //                message = "You can only access your own profile"
        //            });
        //        }
        //        //var user = await userManager.GetUserAsync(User);
        //        var user = await userManager.FindByIdAsync(userId);
        //        if (user == null)
        //        {
        //            return NotFound(new { message = "User not found" });
        //        }
        //        var roles = await userManager.GetRolesAsync(user);

        //        return Ok(new
        //        {
        //            id = user.Id,
        //            email = user.Email,
        //            fullName = user.FullName,
        //            phoneNumber = user.PhoneNumber,
        //            profileImageUrl = user.ProfileImageUrl,
        //            joinDate = user.JoinDate,
        //            isActive = user.IsActive,
        //            roles = roles
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
        //    }
        //}

        ///// <summary>
        ///// Retrieves details for the currently authenticated user
        ///// </summary>
        ///// <returns>Current user details including roles</returns>
        //[HttpGet("getCurrentUser")]
        //[Authorize]
        //[SwaggerOperation(Summary = "Get current user details", Description = "Get current user details")]
        //[SwaggerResponse(StatusCodes.Status200OK, "Current user details retrieved successfully")]
        //[SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        //[SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while retrieving current user")]
        //public async Task<IActionResult> GetCurrentUser()
        //{
        //    try
        //    {
        //        var user = await userManager.GetUserAsync(User);
        //        if (user == null)
        //        {
        //            return NotFound(new { message = "User not found" });
        //        }

        //        var roles = await userManager.GetRolesAsync(user);
        //        return Ok(new
        //        {
        //            id = user.Id,
        //            email = user.Email,
        //            fullName = user.FullName,
        //            phoneNumber = user.PhoneNumber,
        //            profileImageUrl = user.ProfileImageUrl,
        //            joinDate = user.JoinDate,
        //            isActive = user.IsActive,
        //            roles = roles
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred while retrieving current user", error = ex.Message });
        //    }
        //}
    }
}
