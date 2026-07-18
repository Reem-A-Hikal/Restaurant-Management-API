using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Application.Dtos.OrderDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    /// <summary>Controller for managing order operations</summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        /// <summary>
        /// Initializes a new instance of the OrderController
        /// </summary>
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>Creates a new order for the authenticated customer.</summary>
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Create a new order", Description = "Customer only. UserId is extracted from the JWT token.")]
        [SwaggerResponse(201, "Order created successfully", typeof(OrderDto))]
        [SwaggerResponse(400, "Invalid input data")]
        [SwaggerResponse(403, "Forbidden")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _orderService.CreateOrderAsync(userId, dto);

            return CreatedResponse(
                nameof(GetOrderById),
                new { id = order.OrderId },
                order,
                "Order created successfully");
        }

        /// <summary>Gets an order by ID.</summary>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get order by ID", Description = "Admin/Chef see any order. Customer sees own orders only.")]
        [SwaggerResponse(200, "Order retrieved successfully", typeof(OrderDto))]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);

            if(User.IsInRole("Customer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (order.CustomerId != userId)
                    return ForbiddenResponse();
            }

            return SuccessResponse(order);
        }

        /// <summary>Gets orders scoped to the caller's role.</summary>
        [HttpGet]
        [Authorize(Roles = "Admin, Chef")]
        [SwaggerOperation(
            Summary = "Get orders (role-scoped)",
            Description = "Admin sees all orders. Chef sees only Confirmed, Preparing, and Ready orders.")]
        [SwaggerResponse(200, "Orders retrieved successfully", typeof(IEnumerable<OrderDto>))]
        [SwaggerResponse(401, "Unauthorized")]
        [SwaggerResponse(403, "Forbidden")]
        public async Task<IActionResult> GetAllOrders()
        {
            var role = User.FindFirstValue(ClaimTypes.Role)!;
            var orders = await _orderService.GetOrdersVisibleToRoleAsync(role);
            return SuccessResponse(orders);
        }

        /// <summary>Gets orders for a specific customer.</summary>
        [HttpGet("customer/{customerId}")]
        [SwaggerOperation(Summary = "Get orders by customer", Description = "Admin sees any customer orders. Customer sees own orders only.")]
        [SwaggerResponse(200, "Orders retrieved successfully", typeof(IEnumerable<OrderDto>))]
        [SwaggerResponse(403, "Forbidden")]
        public async Task<IActionResult> GetOrdersByCustomer(string customerId)
        {
            if (User.IsInRole("Customer"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                if (userId != customerId)
                    return ForbiddenResponse();
            }

            var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
            return SuccessResponse(orders);
        }

        /// <summary>Gets the authenticated customer's orders.</summary>
        [HttpGet("my-orders")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Get my orders", Description = "Returns orders for the authenticated customer.")]
        [SwaggerResponse(200, "Orders retrieved successfully", typeof(IEnumerable<OrderDto>))]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var orders = await _orderService.GetOrdersByCustomerAsync(userId);
            return SuccessResponse(orders);
        }

        /// <summary>Gets the order statuses the caller's role is allowed to filter by.</summary>
        [HttpGet("allowed-statuses")]
        [Authorize(Roles = "Admin,Chef")]
        [SwaggerOperation(
            Summary = "Get allowed order statuses for current role",
            Description = "Used by the frontend to build the status filter dropdown dynamically.")]
        [SwaggerResponse(200, "Statuses retrieved successfully", typeof(IEnumerable<OrderStatus>))]
        public IActionResult GetAllowedStatuses()
        {
            var role = User.FindFirstValue(ClaimTypes.Role)!;
            var statuses = _orderService.GetAllowedStatusesForRole(role);
            return SuccessResponse(statuses);
        }

        /// <summary>Gets orders by status.</summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Chef")]
        [SwaggerOperation(Summary = "Get orders by status", Description = "Admin sees all statuses. Chef sees Confirmed, Preparing, Ready only.")]
        [SwaggerResponse(200, "Orders retrieved successfully", typeof(IEnumerable<OrderDto>))]
        [SwaggerResponse(403, "Forbidden")]
        public async Task<IActionResult> GetOrdersByStatus(OrderStatus status)
        {
            if (User.IsInRole("Chef"))
            {
                var allowedStatuses = new[] { OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Ready };
                if (!allowedStatuses.Contains(status))
                    return ForbiddenResponse("Chefs can only view Confirmed, Preparing, and Ready orders.");
            }

            var orders = await _orderService.GetOrdersByStatusAsync(status);
            return SuccessResponse(orders);
        }

        /// <summary>Confirms an order.</summary>
        [HttpPatch("{id}/confirm")]
        [Authorize(Roles = "Admin,Chef")]
        [SwaggerOperation(Summary = "Confirm an order", Description = "Admin/Chef only. Transitions order from New to Confirmed.")]
        [SwaggerResponse(200, "Order confirmed successfully", typeof(OrderDto))]
        [SwaggerResponse(400, "Invalid transition")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> ConfirmOrder(int id, [FromBody] ConfirmOrderDto dto)
        {
            var confirmedById = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _orderService.ConfirmOrderAsync(id, confirmedById, dto);

            return SuccessResponse(order, "Order confirmed successfully");

        }

        /// <summary>Cancels an order.</summary>
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = "Admin,Customer")]
        [SwaggerOperation(Summary = "Cancel an order", Description = "Admin cancels any order. Customer cancels own orders only.")]
        [SwaggerResponse(200, "Order cancelled successfully", typeof(OrderDto))]
        [SwaggerResponse(400, "Invalid transition")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> CancelOrder(int id, [FromBody] CancelOrderDto dto)
        {
            var isCustomer = User.IsInRole("Customer");
            if (isCustomer)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order.CustomerId != userId)
                    return ForbiddenResponse();
            }

            var cancelledOrder = await _orderService.CancelOrderAsync(id, dto.CancellationReason, isCustomer);
            return SuccessResponse(cancelledOrder, "Order cancelled successfully");
        }

        /// <summary>Marks an order as preparing.</summary>
        [HttpPatch("{id}/preparing")]
        [Authorize(Roles = "Admin,Chef")]
        [SwaggerOperation(Summary = "Mark order as preparing", Description = "Transitions order from Confirmed to Preparing.")]
        [SwaggerResponse(200, "Order is now being prepared", typeof(OrderDto))]
        [SwaggerResponse(400, "Invalid transition")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> MarkAsPreparing(int id)
        {
            var order = await _orderService.MarkAsPreparingAsync(id);
            return SuccessResponse(order, "Order is now being prepared");
        }

        /// <summary>Marks an order as prepared/ready.</summary>
        [HttpPatch("{id}/prepared")]
        [Authorize(Roles = "Admin,Chef")]
        [SwaggerOperation(Summary = "Mark order as prepared", Description = "Transitions order from Preparing to Ready.")]
        [SwaggerResponse(200, "Order is ready for delivery", typeof(OrderDto))]
        [SwaggerResponse(400, "Invalid transition")]
        [SwaggerResponse(404, "Order not found")]
        public async Task<IActionResult> MarkAsPrepared(int id)
        {
            var order = await _orderService.MarkAsPreparedAsync(id);
            return SuccessResponse(order, "Order is ready for delivery");
        }

        /// <summary>Gets the kitchen queue.</summary>
        [HttpGet("kitchen-queue")]
        [Authorize(Roles = "Admin,Chef")]
        [SwaggerOperation(Summary = "Get kitchen queue", Description = "Returns Confirmed and Preparing orders ordered by date.")]
        [SwaggerResponse(200, "Kitchen queue retrieved successfully", typeof(IEnumerable<OrderDto>))]
        public async Task<IActionResult> GetKitchenQueue()
        {
            var orders = await _orderService.GetKitchenQueueAsync();
            return SuccessResponse(orders);
        }

        /// <summary>Adds an item to an order.</summary>
        [HttpPost("{id}/items")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Add item to order", Description = "Customer only. Order must be in New status.")]
        [SwaggerResponse(200, "Item added successfully", typeof(OrderDto))]
        [SwaggerResponse(400, "Invalid operation")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Order or product not found")]
        public async Task<IActionResult> AddOrderItem(int id, [FromBody] CreateOrderDetailDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order.CustomerId != userId)
                return ForbiddenResponse();

            var updatedOrder = await _orderService.AddOrderItemAsync(id, dto);
            return SuccessResponse(updatedOrder, "Item added successfully");
        }

        /// <summary>Removes an item from an order.</summary>
        [HttpDelete("{id}/items/{productId}")]
        [Authorize(Roles = "Customer")]
        [SwaggerOperation(Summary = "Remove item from order", Description = "Customer only. Order must be in New status.")]
        [SwaggerResponse(200, "Item removed successfully", typeof(OrderDto))]
        [SwaggerResponse(400, "Invalid operation")]
        [SwaggerResponse(403, "Forbidden")]
        [SwaggerResponse(404, "Order or product not found")]
        public async Task<IActionResult> RemoveOrderItem(int id, int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order.CustomerId != userId)
                return ForbiddenResponse();

            var updatedOrder = await _orderService.RemoveOrderItemAsync(id, productId);
            return SuccessResponse(updatedOrder, "Item removed successfully");
        }

        /// <summary>Gets orders within a date range.</summary>
        [HttpGet("date-range")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get orders by date range", Description = "Admin only. Returns orders between startDate and endDate.")]
        [SwaggerResponse(200, "Orders retrieved successfully", typeof(IEnumerable<OrderDto>))]
        [SwaggerResponse(400, "Invalid date range")]
        public async Task<IActionResult> GetOrdersByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return ValidationErrorResponse(new[] { "Start date cannot be after end date" });

            var orders = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate);
            return SuccessResponse(orders);
        }

        /// <summary>Gets daily revenue for a specific date.</summary>
        [HttpGet("revenue/{date}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get daily revenue", Description = "Admin only. Returns total revenue for the specified date.")]
        [SwaggerResponse(200, "Revenue retrieved successfully", typeof(decimal))]
        public async Task<IActionResult> GetDailyRevenue(DateTime date)
        {
            var revenue = await _orderService.GetDailyRevenueAsync(date);
            return SuccessResponse(revenue);
        }

        /// <summary>Gets order count by status.</summary>
        [HttpGet("count/{status}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Get order count by status", Description = "Admin only. Returns count of orders with the specified status.")]
        [SwaggerResponse(200, "Count retrieved successfully", typeof(int))]
        public async Task<IActionResult> GetOrderCountByStatus(OrderStatus status)
        {
            var count = await _orderService.GetOrderCountByStatusAsync(status);
            return SuccessResponse(count);
        }
    }
}
