using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.PaymentDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Services;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Admin fallback: records cash collected for a delivered order.
        /// Normally this happens automatically when the courier marks the order delivered.
        /// </summary>
        [HttpPost("orders/{orderId}/cash")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Record cash payment", Description = "Admin only. Amount is taken from the order, never from the request.")]
        [SwaggerResponse(200, "Cash payment recorded", typeof(PaymentDto))]
        [SwaggerResponse(400, "Order not delivered or already paid")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> RecordCashPayment(int orderId)
        {
            var payment = await _paymentService.RecordCashPaymentAsync(orderId);
            return SuccessResponse(payment, "Cash payment recorded successfully");
        }

        /// <summary>
        /// Refunds a completed payment for an order.
        /// </summary>
        [HttpPost("orders/{orderId}/refund")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Refund payment", Description = "Admin only. Refunds the completed payment for the specified order.")]
        [SwaggerResponse(200, "Payment refunded successfully", typeof(PaymentDto))]
        [SwaggerResponse(400, "No completed payment found")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> RefundPayment(
            int orderId,
            [FromQuery, StringLength(500)] string? gatewayResponse = null)
        {
            var payment = await _paymentService.RefundPaymentAsync(orderId, gatewayResponse);
            return SuccessResponse(payment, "Payment refunded successfully");
        }

        /// <summary>
        /// Gets payment history for an order.
        /// </summary>
        [HttpGet("orders/{orderId}")]
        [Authorize(Roles = "Admin,Customer")]
        [SwaggerOperation(Summary = "Get payment history", Description = "Admin: any order. Customer: own orders only.")]
        [SwaggerResponse(200, "Payment history retrieved successfully", typeof(IEnumerable<PaymentDto>))]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> GetPaymentHistory(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Admin");

            var payments = await _paymentService.GetPaymentHistoryAsync(orderId, userId, isAdmin);
            return SuccessResponse(payments, "Payment history retrieved successfully");
        }
    }
}
