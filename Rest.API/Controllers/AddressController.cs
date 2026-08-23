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
    /// All routes are nested under the owning user and protected by the
    /// SelfOrAdmin policy: the user themselves or an Admin acting on their behalf.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SelfOrAdmin")]
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
        /// Gets all addresses for a specific user
        /// </summary>
        /// <param name="userId">The ID of the address owner</param>
        [HttpGet("user/{userId}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [SwaggerOperation(Summary = "Get user addresses", Description = "Returns all addresses for the specified user.")]
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
        /// <param name="userId">The ID of the address owner</param>
        /// <param name="addressId">Address ID</param>
        [HttpGet("user/{userId}/{addressId}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [SwaggerOperation(Summary = "Get address by ID", Description = "Returns a specific address belonging to the specified user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the requested address")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAddress(string userId,int addressId)
        {
            var address = await _addressService.GetUserAddress(userId, addressId);

            if (address == null)
                return NotFoundResponse("Address not found.");

            return SuccessResponse(address, "Address retrieved successfully");
        }

        /// <summary>
        /// Gets the default address for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the address owner</param>
        [HttpGet("user/{userId}/default")]
        [Authorize(Policy = "SelfOrAdmin")]
        [SwaggerOperation(Summary = "Get default address", Description = "Returns the default address for the currently authenticated user.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the default address")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "No default address found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDefaultAddress(string userId)
        {
            var address = await _addressService.GetUserDefaultAddress(userId!);

            if (address == null)
                return NotFoundResponse("No default address found.");

            return SuccessResponse(address, "Default address retrieved successfully");
        }

        /// <summary>
        /// Creates a new address for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the address owner</param>
        /// <param name="dto">Address data</param>
        [HttpPost("user/{userId}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [SwaggerOperation(Summary = "Create address", Description = "Creates a new address for the currently authenticated user. The first address created is automatically set as default.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Address created successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid address data")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateAddress(string userId, [FromBody] AddressCreateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var created = await _addressService.CreateAddress(userId, dto);

            return CreatedResponse(
                nameof(GetAddress),
                new { userId, addressId = created.AddressId },
                created,
                "Address created successfully");
        }

        /// <summary>
        /// Updates an existing address belonging to the specified user.
        /// </summary>
        /// <param name="userId">The ID of the address owner</param>
        /// <param name="addressId">Address ID</param>
        /// <param name="dto">Updated address data</param>
        [HttpPut("user/{userId}/{addressId}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [SwaggerOperation(Summary = "Update address", Description = "Updates an existing address belonging to the specified user.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Address updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid address data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateAddress(string userId, int addressId, [FromBody] AddressUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            await _addressService.UpdateAddress(userId!, addressId, dto);

            return NoContent();
        }

        /// <summary>
        /// Sets an address as the default address for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the address owner</param>
        /// <param name="addressId">Address ID to set as default</param>
        [HttpPatch("user/{userId}/{addressId}/set-default")]
        [Authorize(Policy = "SelfOrAdmin")]
        [SwaggerOperation(Summary = "Set default address", Description = "Sets the specified address as the default address. Unsets any previously default address.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Default address set successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> SetDefaultAddress(string userId, int addressId)
        {
            await _addressService.SetDefaultAddress(userId!, addressId);

            return NoContent();
        }


        /// <summary>
        /// Deletes an address belonging to the specified user.
        /// </summary>
        /// <param name="userId">The ID of the address owner</param>
        /// <param name="addressId">Address ID</param>
        [HttpDelete("user/{userId}/{addressId}")]
        [Authorize(Policy = "SelfOrAdmin")]
        [SwaggerOperation(Summary = "Delete address", Description = "Deletes an address belonging to the specified user. If the deleted address was the default, another address is automatically set as default.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Address deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Address not found")]
        [SwaggerResponse(StatusCodes.Status401Unauthorized)]
        [SwaggerResponse(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAddress(string userId, int addressId)
        {
            await _addressService.DeleteAddressAsync(userId, addressId);

            return NoContent();
        }
    }
}
