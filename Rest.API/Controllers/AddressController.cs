using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.AddressDtos;
using Rest.Application.Interfaces.IServices;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing user addresses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AddressController : BaseController
    {
        private readonly IAddressService _addressService;

        /// <summary>
        /// Initializes a new instance of the AddressController
        /// </summary>
        public AddressController(IAddressService address, ILogger<AddressController> logger, IUserService userService)
        {
            _addressService = address;
        }

        /// <summary>
        /// Gets all addresses for the currently authenticated user
        /// </summary>
        [HttpGet("my-addresses")]
        [SwaggerOperation(Summary = "Get my addresses", Description = "Returns all addresses belonging to the currently authenticated user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of addresses")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMyAddresses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var addresses = await _addressService.GetUserAddresses(userId!);
            return SuccessResponse(addresses, "Addresses retrieved successfully");
        }

        /// <summary>
        /// Gets all addresses for a specific user (Admin only)
        /// </summary>
        /// <param name="userId">The ID of the user whose addresses to retrieve</param>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get user addresses (Admin)", Description = "Requires Admin role. Returns all addresses for the specified user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns list of addresses")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserAddresses(string userId)
        {
            var addresses = await _addressService.GetUserAddresses(userId);
            return SuccessResponse(addresses, "Addresses retrieved successfully");
        }

        /// <summary>
        /// Gets a specific address belonging to the current user
        /// </summary>
        /// <param name="id">Address ID</param>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get address by ID", Description = "Returns a specific address belonging to the currently authenticated user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested address")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAddress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var address = await _addressService.GetUserAddress(userId!, id);

            if (address == null)
                return NotFoundResponse("Address not found.");

            return SuccessResponse(address, "Address retrieved successfully");
        }

        /// <summary>
        /// Gets the default address for the currently authenticated user
        /// </summary>
        [HttpGet("default")]
        [SwaggerOperation(Summary = "Get default address", Description = "Returns the default address for the currently authenticated user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the default address")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No default address found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDefaultAddress()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var address = await _addressService.GetUserDefaultAddress(userId!);

            if (address == null)
                return NotFoundResponse("No default address found.");

            return SuccessResponse(address, "Default address retrieved successfully");
        }

        /// <summary>
        /// Creates a new address for the currently authenticated user
        /// </summary>
        /// <param name="dto">Address data</param>
        [HttpPost]
        [SwaggerOperation(Summary = "Create address", Description = "Creates a new address for the currently authenticated user. The first address created is automatically set as default.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Address created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid address data")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateAddress([FromBody] AddressCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var created = await _addressService.CreateAddress(userId!, dto);

            return CreatedResponse(nameof(GetAddress), new { id = created.AddressId }, created, "Address created successfully");
        }

        /// <summary>
        /// Updates an existing address belonging to the current user
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <param name="dto">Updated address data</param>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update address", Description = "Updates an existing address belonging to the currently authenticated user.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Address updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid address data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _addressService.UpdateAddress(userId!, id, dto);

            return NoContent();
        }

        /// <summary>
        /// Sets an address as the default address for the current user
        /// </summary>
        /// <param name="id">Address ID to set as default</param>
        [HttpPatch("{id}/set-default")]
        [SwaggerOperation(Summary = "Set default address", Description = "Sets the specified address as the default address. Unsets any previously default address.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Default address set successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _addressService.SetDefaultAddress(userId!, id);

            return NoContent();
        }


        /// <summary>
        /// Deletes an address belonging to the current user
        /// </summary>
        /// <param name="id">Address ID</param>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete address", Description = "Deletes an address belonging to the currently authenticated user. If the deleted address was the default, another address is automatically set as default.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Address deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _addressService.DeleteAddressAsync(userId!, id);

            return NoContent();
        }
    }
}
