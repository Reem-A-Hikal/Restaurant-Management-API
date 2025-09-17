using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices;
using Swashbuckle.AspNetCore.Annotations;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing user operations including retrieval, updates, and deletion
    /// </summary>
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Controller for managing user operations including retrieval, updates, and deletion
        /// </summary>
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Initializes a new instance of the UserController
        /// </summary>
        /// <param name="userService">The user service for business logic operations</param>
        /// <param name="logger">The logger for logging operations</param>
        public UserController(IUserService userService,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        /// <summary>
        /// Retrieves all users in the system (Admin only)
        /// </summary>
        /// <returns>A list of all users</returns>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Retrieves a list of all users in the system. Requires Admin role.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of users")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { Message = $"An error occurred while retrieving users ${ex.Message}" });
            }
        }

        /// <summary>
        /// Retrieves all users in the system (Admin only)
        /// </summary>
        /// <param name="pageIndex">the index of current Page</param>
        /// <param name="pageSize">the count of users to display in the page</param>
        /// <param name="searchTerm"></param>
        /// <param name="selectedRole"></param>
        /// <returns>A list of all users</returns>
        [HttpGet("GetAllPaginated")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Get all users",
            Description = "Retrieves a list of all users in the system. Requires Admin role.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of users")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetAllUsers ( [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 6, [FromQuery] string? searchTerm = "", [FromQuery] string? selectedRole = "")
        {
            try
            {
                var users = await _userService.GetPaginatedUsersWithFilterAsync( pageIndex, pageSize, searchTerm, selectedRole );
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { Message = $"An error occurred while retrieving users${ex.Message}" });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="userId">The user ID to retrieve</param>
        [Authorize(Policy = "SelfOrAdmin")]
        [HttpGet("GetUserById/{userId}")]
        [SwaggerOperation(
            Summary = "Get user by ID",
            Description = "Retrieves user details by ID. Users can only access their own profile unless they are Admins.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns user details")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Cannot access other users' data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found");
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", userId);
                return StatusCode(500, new { Message = $"An error occurred while retrieving the user ${ex.Message}" });
            }
        }
        /// <summary>
        /// Create User with specefic role
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Confirmation of successful creation</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(CreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }
            try
            {
                var id = await _userService.AddUser(model);
                return CreatedAtAction(nameof(GetUserById), new { userId = id }, new { Message = "User created successfully", UserId = id });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating user");
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { Message = $"An error occurred while creating the user{ex.Message}" });
            }
        }

        /// <summary>
        /// Updates a user's profile information
        /// </summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <param name="updateDto">The updated profile data</param>
        /// <returns>Confirmation of successful update</returns>
        /// <remarks>
        /// Special fields are role-specific:
        /// - Chef: Can update specialization
        /// - DeliveryPerson: Can update vehicleNumber
        /// Users can only update their own profile unless they are Admins
        /// </remarks>
        [Authorize(Policy = "SelfOrAdmin")]
        [HttpPut("UpdateProfile/{userId}")]
        [SwaggerOperation(
            Summary = "Update user profile",
            Description = "Updates user profile information. Special fields are role-specific:\n" +
                         "- Chef: Can update specialization\n" +
                         "- DeliveryPerson: Can update vehicleNumber")]
        [SwaggerResponse(StatusCodes.Status200OK, "Profile updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Cannot update other users' data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UpdateProfile(string userId, [FromBody] UpdateProfileDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                });
            }
            try
            {
                await _userService.UpdateUserProfileAsync(userId, updateDto);
                return Ok(new { Message = "Profile updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with ID: {UserId}", userId);
                return NotFound(new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Validation error for user ID: {UserId}", userId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user ID: {UserId}", userId);
                return StatusCode(500, new { Message = $"An error occurred while updating the profile ${ex.Message}" });
            }
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        /// <param name="userId">The user ID to delete</param>
        [HttpDelete("DeleteUser/{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Delete user",
            Description = "Deletes a user account. Requires Admin role.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "User deleted successfully")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized access")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden - Admin role required")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                await _userService.DeleteUser(userId);
                return Ok(new { Message = "User deleted successfully", userId });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with ID: {UserId}", userId);
                return NotFound(new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Validation error for user ID: {UserId}", userId);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", userId);
                return StatusCode(500, new { Message = $"An error occurred while deleting the user ${ex.Message}" });
            }
        }
    }
}
