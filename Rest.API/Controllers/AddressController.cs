using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.AddressDtos;
using Rest.Application.Interfaces.IServices;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    /// <summary>
    /// Controller for managing user addresses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AddressController> _logger;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor for AddressController.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="logger"></param>
        /// <param name="userService"></param>
        public AddressController(IAddressService address, ILogger<AddressController> logger, IUserService userService)
        {
            _addressService = address;
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Creates a new address for the user.
        /// </summary>
        /// <returns> The created address.</returns>
        [HttpGet("UserAddressesSelf")]
        public async Task<IActionResult> GetUserAddressesforhimSelf()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var addresses = await _addressService.GetUserAddresses(userId);
                if (addresses == null || !addresses.Any())
                {
                    return NotFound("No addresses found for the user.");
                }
                return Ok(addresses);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user addresses.");
                return StatusCode(500, "An error occurred while retrieving user addresses." + ex);
            }
        }

        [HttpGet("UserAddressesAdmin/{userid}")]
        public async Task<IActionResult> GetUserAddressesAdmin(string userid)
        {
            try
            {
                var addresses = await _addressService.GetUserAddresses(userid);
                if (addresses == null || !addresses.Any())
                {
                    return NotFound("No addresses found for the user.");
                }
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user addresses.");
                return StatusCode(500, "An error occurred while retrieving user addresses." + ex);
            }
        }
        /// <summary>
        /// Get a specific address by ID
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <returns>Address details</returns>
        [HttpGet("UserAddress/{id}")]
        public async Task<IActionResult> GetUserAddress(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var address = await _addressService.GetUserAddress(userId, id);
                if (address == null)
                {
                    return NotFound("Address not found.");
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the address.");
                return StatusCode(500, "An error occurred while retrieving the address.");
            }
        }

        /// <summary>
        /// Get the user's default address
        /// </summary>
        /// <returns>Default address details</returns>
        [HttpGet("DefaultAddress")]
        public async Task<IActionResult> GetUserDefaultAddress()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var address = await _addressService.GetUserDefaultAddress(userId);
                if (address == null)
                {
                    return NotFound("Default address not found.");
                }
                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the default address.");
                return StatusCode(500, "An error occurred while retrieving the default address.");
            }
        }

        /// <summary>
        /// Create a new address
        /// </summary>
        /// <param name="addressDto">Address data</param>
        /// <returns>Created address</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateAddress([FromBody] AddressCreateDto addressDto)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var CreatedAddress = await _addressService.CreateAddress(userId, addressDto);
                if (CreatedAddress == null)
                {
                    return BadRequest("Failed to create address.");
                }
                return CreatedAtAction(nameof(GetUserAddress), new { id = CreatedAddress.AddressId }, CreatedAddress);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the address.");
                return StatusCode(500, "An error occurred while creating the address.");
            }
        }

        /// <summary>
        /// Update an existing address
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <param name="addressDto">Updated address data</param>
        /// <returns>Update result</returns>
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressUpdateDto addressDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _addressService.UpdateAddress(userId, id, addressDto);
                if (!result)
                {
                    return NotFound("Address not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the address.");
                return StatusCode(500, "An error occurred while updating the address.");
            }
        }

        /// <summary>
        /// Delete an address
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <returns>Delete result</returns>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _addressService.DeleteAddressAsync(userId, id);
                if (!result)
                {
                    return NotFound("Address not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the address.");
                return StatusCode(500, "An error occurred while deleting the address.");
            }
        }
    }
}
