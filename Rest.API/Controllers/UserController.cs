using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IServices;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing user operations including retrieval, updates, and deletion
    /// </summary>
    [Route("api/user")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        /// <summary>
        /// Controller for managing user operations including retrieval, updates, and deletion
        /// </summary>
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the UserController
        /// </summary>
        /// <param name="userService">The user service for business logic operations</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// Retrieves all users without pagination (Admin only)
        /// </summary>
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get all users", Description = "Requires Admin role.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of users")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return SuccessResponse(users, "Users retrieved successfully");
        }

        /// <summary>
        /// Retrieves paginated users with optional filtering (Admin only)
        /// </summary>
        /// <param name="pageIndex">the index of current Page</param>
        /// <param name="pageSize">the count of users to display in the page</param>
        /// <param name="searchTerm"></param>
        /// <param name="selectedRole"></param>
        /// <returns>A list of all users</returns>
        [HttpGet("GetAllPaginated")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get paginated users")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns paginated list of users")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers ( 
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 6,
            [FromQuery] string? searchTerm = "",
            [FromQuery] string? selectedRole = "")
        {
            var users = await _userService.GetPaginatedUsersWithFilterAsync( 
                pageIndex, pageSize, searchTerm, selectedRole );

            return SuccessResponse(users, "Users retrieved successfully");
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
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            return SuccessResponse(user, "User retrieved successfully");
        }
        /// <summary>
        /// Create User with specefic role
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Confirmation of successful creation</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Create user")]
        [SwaggerResponse(StatusCodes.Status201Created, "User created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid data")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
            
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var id = await _userService.AddUser(model);
            return CreatedResponse(
                nameof(GetUserById),
                new { userId = id },
                new { userId = id },
                "User created successfully");
        }

        /// <summary>
        /// Admin update - IsActive, Role, Chef specialization, DeliveryPerson vehicle
        /// </summary>
        [HttpPut("AdminUpdate/{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(
            Summary = "Admin update user",
            Description = "Admin can update: IsActive, Specialization (Chef), VehicleNumber & IsAvailable (DeliveryPerson)")]
        [SwaggerResponse(StatusCodes.Status200OK, "User updated successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "User not found")]
        public async Task<IActionResult> AdminUpdateUser(string userId, [FromBody] AdminUpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            await _userService.AdminUpdateUserAsync(userId, dto);
            return SuccessResponse<string>(null, "User updated successfully");
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
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));
        
            await _userService.UpdateUserProfileAsync(userId, updateDto);
            return SuccessResponse<string>(null, "Profile updated successfully");
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
            await _userService.DeleteUser(userId);
            return SuccessResponse(userId, "User deleted successfully");
        }
    }
}
