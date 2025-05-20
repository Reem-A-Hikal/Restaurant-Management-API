using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Rest.API.Dtos.UserDtos;
using Rest.API.Models;
using Rest.API.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

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
        ILogger<UserController> _logger;

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
        /// 
        [HttpGet("all")]
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
                return StatusCode(500, new { Message = "An error occurred while retrieving users" });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="userId">The user ID to retrieve</param>
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
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                if (currentUserId != userId && !isAdmin)
                {
                    return Forbid("You are not authorized to access this resource");
                }
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user with ID: {UserId}", userId);
                return StatusCode(500, new { Message = "An error occurred while retrieving the user" });
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                var currentUserRoles = User.FindFirstValue(ClaimTypes.Role)?.Split(",") ?? Array.Empty<string>();


                if (currentUserId != userId && !isAdmin)
                {
                    _logger.LogWarning($"Unauthorized update attempt by {currentUserId} for user {userId}");
                    return Forbid("You don't have permission to update this user profile");
                }

                if (!currentUserRoles.Contains("Chef"))
                {
                    updateDto.Specialization = null;
                }

                if (!currentUserRoles.Contains("DeliveryPerson"))
                {
                    updateDto.VehicleNumber = null;
                }

                await _userService.UpdateUserProfileAsync(userId, updateDto);
                return Ok("profile updated successfully");
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
                return StatusCode(500, new { Message = "An error occurred while updating the profile" });
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
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found with ID: {UserId}", userId);
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", userId);
                return StatusCode(500, new { Message = "An error occurred while deleting the user" });
            }
        }
    }
}
