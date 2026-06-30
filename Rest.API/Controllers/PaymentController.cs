using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.PaymentDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;

        public PaymentController(IPaymentService paymentService, IOrderService orderService)
        {
            _paymentService = paymentService;
            _orderService = orderService;
        }

        /// <summary>
        /// Processes a payment for an order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("orders/{orderId}")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Process payment", Description = "Customer only. Creates a payment for the specified order.")]
        [SwaggerResponse(200, "Payment processed successfully", typeof(PaymentDto))]
        [SwaggerResponse(400, "Invalid payment or dublicate completed payment")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> ProcessPayment(int orderId, [FromBody] ProcessPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order.CustomerId != currentUserId)
                return ForbiddenResponse("You can only pay for your own orders.");

            var payment = await _paymentService.ProcessPaymentAsync(orderId, dto);
            return SuccessResponse(payment, "Payment processed successfully");
        }
        /// <summary>
        /// Refunds a completed payment for an order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="gatewayResponse"></param>
        /// <returns></returns>
        [HttpPost("orders/{orderId}/refund")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Refund payment", Description = "Admin only. Refunds the completed payment for the specified order.")]
        [SwaggerResponse(200, "Payment refunded successfully", typeof(PaymentDto))]
        [SwaggerResponse(400, "No completed payment found")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> RefundPayment(int orderId, [FromQuery] string? gatewayResponse = null)
        {
            var payment = await _paymentService.RefundPaymentAsync(orderId, gatewayResponse);
            return SuccessResponse(payment, "Payment refunded successfully");
        }

        /// <summary>
        /// Gets payment history for an order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("orders/{orderId}")]
        [Authorize(Roles = "Admin, Customer")]
        [SwaggerOperation(Summary = "Get payment history", Description = "Admin or Customer. Returns all payment attempts for the specified order.")]
        [SwaggerResponse(200, "Payment history retrieved successfully", typeof(IEnumerable<PaymentDto>))]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> GetPaymentHistory(int orderId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Customer")
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order.CustomerId != currentUserId)
                    return ForbiddenResponse("You can only view your own payments.");
            }

            var payments = await _paymentService.GetPaymentHistoryAsync(orderId);
            return SuccessResponse(payments, "Payment history retrieved successfully");
        }
    }
}
