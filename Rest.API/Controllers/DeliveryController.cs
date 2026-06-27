using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.DeliveryDtos;
using Rest.Application.Interfaces.IServices;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DeliveryController : BaseController
    {
        private readonly IDeliveryService _deliveryService;

        public DeliveryController(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        /// <summary>
        /// Assign a delivery person to a Ready order (auto or manual override)
        /// </summary>
        [HttpPost("assign/{orderId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignDelivery(int orderId, [FromBody] AssignDeliveryDto dto)
        {
            var result = await _deliveryService.AssignDeliveryAsync(orderId, dto);
            return SuccessResponse(result, "Delivery assigned successfully");
        }

        /// <summary>
        /// DeliveryPerson marks order as picked up
        /// </summary>
        [HttpPut("{deliveryId}/pickup")]
        [Authorize(Roles = "DeliveryPerson")]
        public async Task<IActionResult> MarkAsPickedUp(int deliveryId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activeDelivery = await _deliveryService.GetActiveDeliveryForOrderAsync(deliveryId);
            if (activeDelivery == null)
                return NotFoundResponse("Delivery not found.");

            if (activeDelivery.DeliveryPersonId != currentUserId)
                return ForbiddenResponse("You can only update your own deliveries.");

            var result = await _deliveryService.MarkAsPickedUpAsync(deliveryId);
            return SuccessResponse(result, "Order marked as picked up");
        }

        /// <summary>
        /// DeliveryPerson marks order as delivered
        /// </summary>
        [HttpPut("{deliveryId}/delivered")]
        [Authorize(Roles = "DeliveryPerson")]
        public async Task<IActionResult> MarkAsDelivered(int deliveryId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var activeDelivery = await _deliveryService.GetActiveDeliveryForOrderAsync(deliveryId);
            if (activeDelivery == null)
                return NotFoundResponse("Delivery not found.");

            if (activeDelivery.DeliveryPersonId != currentUserId)
                return ForbiddenResponse("You can only update your own deliveries.");

            var result = await _deliveryService.MarkAsDeliveredAsync(deliveryId);
            return SuccessResponse(result, "Order marked as delivered");
        }

        /// <summary>
        /// Cancel an active delivery — Admin, Chef, or the assigned DeliveryPerson
        /// </summary>
        [HttpPut("{deliveryId}/cancel")]
        [Authorize(Roles = "Admin,DeliveryPerson")]
        public async Task<IActionResult> CancelDelivery(int deliveryId, [FromBody] CancelDeliveryDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "DeliveryPerson")
            {
                var activeDelivery = await _deliveryService.GetActiveDeliveryForOrderAsync(deliveryId);
                if (activeDelivery == null)
                    return NotFoundResponse("Delivery not found.");

                if (activeDelivery.DeliveryPersonId != currentUserId)
                    return ForbiddenResponse("You can only cancel your own deliveries.");
            }

            var result = await _deliveryService.CancelDeliveryAsync(deliveryId, dto.Reason);
            return SuccessResponse(result, "Delivery cancelled successfully");
        }

        /// <summary>
        /// Get active delivery for a specific order
        /// </summary>
        [HttpGet("order/{orderId}/active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActiveDeliveryForOrder(int orderId)
        {
            var result = await _deliveryService.GetActiveDeliveryForOrderAsync(orderId);
            if (result == null)
                return NotFoundResponse("No active delivery found for this order.");

            return SuccessResponse(result, "Active delivery retrieved successfully");
        }

        /// <summary>
        /// Get full delivery history for an order
        /// </summary>
        [HttpGet("order/{orderId}/history")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDeliveryHistory(int orderId)
        {
            var result = await _deliveryService.GetDeliveryHistoryAsync(orderId);
            return SuccessResponse(result, "Delivery history retrieved successfully");
        }

        /// <summary>
        /// DeliveryPerson gets their own active deliveries
        /// </summary>
        [HttpGet("my-deliveries")]
        [Authorize(Roles = "DeliveryPerson")]
        public async Task<IActionResult> GetMyDeliveries()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _deliveryService.GetMyDeliveriesAsync(currentUserId!);
            return SuccessResponse(result, "Your active deliveries retrieved successfully");
        }

        /// <summary>
        /// Get all active deliveries in the system — Admin dashboard
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllActiveDeliveries()
        {
            var result = await _deliveryService.GetAllActiveDeliveriesAsync();
            return SuccessResponse(result, "All active deliveries retrieved successfully");
        }
    }
}

