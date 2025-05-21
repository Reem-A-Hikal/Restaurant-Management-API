using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rest.API.Dtos.AccountDtos;
using Rest.API.Dtos.UserDtos;
using Rest.API.Models;
using Rest.API.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for handling user authentication and account management operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper mapper;
        private readonly SignInManager<User> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IAuthService authService;

        /// <summary>
        /// Initializes a new instance of the AccountController
        /// </summary>
        /// <param name="userManager">Provides the APIs for managing user in a persistence store</param>
        /// <param name="mapper">AutoMapper instance for object-object mapping</param>
        /// <param name="signInManager">Manages user sign-in operations</param>
        /// <param name="roleManager">Provides the APIs for managing roles in a persistence store</param>
        /// <param name="authService">Service for authentication related operations</param>
        public AccountController(
            UserManager<User> userManager,
            IMapper mapper,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
             IAuthService authService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.signInManager = signInManager;
            this.authService = authService;
            this.roleManager = roleManager;
        }

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        /// <returns>Registration result with token and user details</returns>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/account/register
        ///     {
        ///         "email": "user@example.com",
        ///         "password": "Password123!",
        ///         "phoneNumber": "0123456789",
        ///         "fullName": "John Doe"
        ///     }
        /// 
        /// Admins can specify a role during registration
        /// </remarks>
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
                var existingUser = await userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return BadRequest(ModelState);
                }

                var user = mapper.Map<User>(registerDto);

                var result = await userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                {
                    string roleToAssign = "Customer";
                    var token = await authService.GenerateJwtTokenAsync(user);
                    if (User.IsInRole("Admin") && !string.IsNullOrEmpty(registerDto.Role))
                    {
                        var roleExists = await roleManager.RoleExistsAsync(registerDto.Role);
                        if (roleExists)
                        {
                            roleToAssign = registerDto.Role;
                        }
                    }
                    await userManager.AddToRoleAsync(user, roleToAssign);
                    return Ok(new
                    {
                        message = "User registered successfully",
                        token = token,
                        userId = user.Id,
                        email = user.Email,
                        role = roleToAssign
                    });
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
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
                var user = await authService.ValidateUserCredentialsAsync(loginDto.Email, loginDto.Password);
                if (user == null)
                {
                    return BadRequest(new { message = "Invalid email or password" });
                }
                var token = await authService.GenerateJwtTokenAsync(user);
                var userRoles = await userManager.GetRolesAsync(user);
                var userDto = mapper.Map<UserDto>(user);
                userDto.Roles = userRoles.ToList();

                return Ok(new
                {
                    message = "Login successful",
                    token = token,
                    email = userDto.Email,
                    roles = userRoles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        /// <summary>
        /// Logs out the currently authenticated user
        /// </summary>
        /// <returns>Logout confirmation</returns>
        [HttpPost("logout")]
        [Authorize]
        [SwaggerOperation(Summary = "Logout a user", Description = "Logout a user")]
        [SwaggerResponse(StatusCodes.Status200OK, "User logged out successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred during logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves details for a specific user
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve</param>
        /// <returns>User details including roles</returns>
        /// <remarks>
        /// Users can only access their own profile unless they are Admins
        /// </remarks>
        [HttpGet("getUser/{userId}")]
        [Authorize]
        [SwaggerOperation(Summary = "Get user details", Description = "Get user details by user ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "User details retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while retrieving user")]
        public async Task<IActionResult> GetUser(string userId)
        {
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId != userId && !User.IsInRole("Admin"))
                {
                    //return Unauthorized();
                    return StatusCode(403, new
                    {
                        message = "You can only access your own profile"
                    });
                }
                //var user = await userManager.GetUserAsync(User);
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                var roles = await userManager.GetRolesAsync(user);

                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    phoneNumber = user.PhoneNumber,
                    profileImageUrl = user.ProfileImageUrl,
                    joinDate = user.JoinDate,
                    isActive = user.IsActive,
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves details for the currently authenticated user
        /// </summary>
        /// <returns>Current user details including roles</returns>
        [HttpGet("getCurrentUser")]
        [Authorize]
        [SwaggerOperation(Summary = "Get current user details", Description = "Get current user details")]
        [SwaggerResponse(StatusCodes.Status200OK, "Current user details retrieved successfully")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Access denied")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while retrieving current user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var roles = await userManager.GetRolesAsync(user);
                return Ok(new
                {
                    id = user.Id,
                    email = user.Email,
                    fullName = user.FullName,
                    phoneNumber = user.PhoneNumber,
                    profileImageUrl = user.ProfileImageUrl,
                    joinDate = user.JoinDate,
                    isActive = user.IsActive,
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving current user", error = ex.Message });
            }
        }
    }
}
