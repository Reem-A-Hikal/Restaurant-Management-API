using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.OrderDtos;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities.Enums;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(OrderDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto orderDto)
        { 
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    _logger.LogWarning("Unauthenticated user attempted to create order");
                    return Unauthorized();
                }
                if (orderDto.UserId != currentUserId)
                {
                    _logger.LogWarning($"User {currentUserId} attempted to create order for another user");
                    return Forbid();
                }
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Customer")
                {
                    return Unauthorized("Only customers can create orders.");
                }
                if (orderDto.OrderDetails == null || orderDto.OrderDetails.Count == 0)
                {
                    return BadRequest("Order must contain at least one item.");
                }

                var order = await _orderService.CreateOrderAsync(orderDto);
                _logger.LogInformation($"Order {order.OrderId} created successfully by user {currentUserId}");

                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Invalid argument in CreateOrder: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "Internal server error occurred" +ex);
            }
        }

        [HttpGet("getOrderById/{id}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Chef" && currentUserId != order.CustomerId)
                    return Forbid("You can only view your own orders");

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Order not found: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order {id}");
                return StatusCode(500, "Internal server error occurred" + ex);
            }
        }
        [HttpGet("GetAllOrders")]
        [Authorize(Roles = "Admin,Chef")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                foreach (var order in orders)
                {
                    order.StatusDisplay = order.Status.ToString(); // Assuming you want to display the enum name
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("Bycustomer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomer(string customerId)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Chef" && currentUserId != customerId)
                    return Forbid("You can only view your own orders");

                var orders = await _orderService.GetOrdersByCustomerAsync(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for customer {customerId}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("Bystatus/{status}")]
        [Authorize(Roles = "Admin,Chef")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(OrderStatus status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                foreach(var order in orders)
                {
                    order.StatusDisplay = order.Status.ToString(); // Assuming you want to display the enum name
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders with status {status}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        //[HttpGet("date-range")]
        //[Authorize(Roles = "Admin,Chef")]
        //[ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByDateRange(
        //    [FromQuery] DateTime startDate,
        //    [FromQuery] DateTime endDate)
        //{
        //    try
        //    {
        //        if (startDate > endDate)
        //            return BadRequest("Start date cannot be after end date");

        //        var orders = await _orderService.GetOrdersByDateRangeAsync(startDate, endDate);
        //        foreach (var order in orders)
        //        {
        //            order.StatusDisplay = order.Status.ToString(); // Assuming you want to display the enum name
        //        }
        //        return Ok(orders);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error retrieving orders between {startDate} and {endDate}");
        //        return StatusCode(500, "Internal server error occurred");
        //    }
        //}

        [HttpGet("kitchen-queue")]
        [Authorize(Roles = "Admin,Chef,Kitchen")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetKitchenQueue()
        {
            try
            {
                var orders = await _orderService.GetKitchenQueueAsync();
                foreach (var order in orders)
                {
                    order.StatusDisplay = order.Status.ToString(); // Assuming you want to display the enum name
                }
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving kitchen queue");
                return StatusCode(500, "Internal server error occurred");
            }
        }
        [HttpGet("pending-delivery")]
        [Authorize(Roles = "Admin,Chef,Delivery")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetPendingDeliveryOrders()
        {
            try
            {
                var orders = await _orderService.GetPendingDeliveryOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending delivery orders");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPatch("{id}/confirm")]
        [Authorize(Roles = "Admin,Chef")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> ConfirmOrder(int id, [FromBody] ConfirmOrderDto confirmDto)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var order = await _orderService.ConfirmOrderAsync(id, currentUserId, confirmDto);

                _logger.LogInformation($"Order {id} confirmed by user {currentUserId}");
                order.StatusDisplay = order.Status.ToString(); // Assuming you want to display the enum name
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning($"Order confirmation failed: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Invalid order confirmation: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming order {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }
        [HttpPatch("{id}/prepared")]
        [Authorize(Roles = "Admin,Chef,Kitchen")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> MarkAsPrepared(int id)
        {
            try
            {
                var order = await _orderService.MarkAsPreparedAsync(id);
                order.StatusDisplay = order.Status.ToString(); // Assuming you want to display the enum name
                _logger.LogInformation($"Order {id} marked as prepared");

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking order {id} as prepared");
                return StatusCode(500, "Internal server error occurred");
            }
        }
        [HttpPatch("{orderid}/assign-delivery")]
        [Authorize(Roles = "Admin,Chef")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> AssignDeliveryPerson(int orderId, [FromBody] AssignDeliveryDto assignDto)
        {
            try
            {
                var order = await _orderService.AssignDeliveryPersonAsync(orderId, assignDto.DeliveryPersonId);

                _logger.LogInformation($"Order {orderId} assigned to delivery person {assignDto.DeliveryPersonId}");
                order.StatusDisplay = order.Status.ToString(); // Assuming you want to display the enum name
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error occurred: ${ex.Message}");
            }
        }

        [HttpPatch("{id}/delivered")]
        [Authorize(Roles = "Admin,Chef,Delivery")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> MarkAsDelivered(int id)
        {
            try
            {
                var order = await _orderService.MarkAsDeliveredAsync(id);

                _logger.LogInformation($"Order {id} marked as delivered");

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking order {id} as delivered");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPatch("{id}/cancel")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> CancelOrder(int id, [FromBody] string cancellationReason)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Chef" && currentUserId != order.CustomerId)
                    return Forbid("You can only cancel your own orders");

                var cancelledOrder = await _orderService.CancelOrderAsync(id, cancellationReason);

                _logger.LogInformation($"Order {id} cancelled by user {currentUserId}");

                return Ok(cancelledOrder);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }
        [HttpPost("{id}/items")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> AddOrderItem(int id, [FromBody] CreateOrderDetailDto itemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                
                var order = await _orderService.GetOrderByIdAsync(id);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Chef" && currentUserId != order.CustomerId)
                    return Forbid("You can only modify your own orders");

                var updatedOrder = await _orderService.AddOrderItemAsync(id, itemDto);

                _logger.LogInformation($"Item added to order {id} by user {currentUserId}");

                return Ok(updatedOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding item to order {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpDelete("{id}/items/{productId}")]
        [ProducesResponseType(typeof(OrderDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderDto>> RemoveOrderItem(int id, int productId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Chef" && currentUserId != order.CustomerId)
                    return Forbid("You can only modify your own orders");

                var updatedOrder = await _orderService.RemoveOrderItemAsync(id, productId);

                _logger.LogInformation($"Item {productId} removed from order {id} by user {currentUserId}");

                return Ok(updatedOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing item from order {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }
        [HttpPost("{id}/payment")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ProcessPayment(int id, [FromBody] PaymentMethod payment)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _orderService.GetOrderByIdAsync(id);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Chef" && currentUserId != order.CustomerId)
                    return Forbid("You can only process payment for your own orders");

                await _orderService.ProcessPaymentAsync(id, payment);

                _logger.LogInformation($"Payment processed for order {id} by user {currentUserId}");

                return Ok(new { message = "Payment processed successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing payment for order {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _orderService.GetOrderByIdAsync(id);
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

                if (userRole != "Admin" && userRole != "Chef" && currentUserId != order.CustomerId)
                    return Forbid("You can only update your own orders");

                await _orderService.UpdateOrderAsync(id, orderDto);

                _logger.LogInformation($"Order {id} updated by user {currentUserId}");

                return Ok(new { message = "Order updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _orderService.DeleteOrderAsync(id);

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"Order {id} deleted by user {currentUserId}");

                return Ok(new { message = "Order deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("stats/count/{status}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<int>> GetOrderCountByStatus(OrderStatus status)
        {
            try
            {
                var count = await _orderService.GetOrderCountByStatusAsync(status);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order count for status {status}");
                return StatusCode(500, "Internal server error occurred");
            }
        }

        [HttpGet("stats/revenue/{date}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(decimal), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<decimal>> GetDailyRevenue(DateTime date)
        {
            try
            {
                var revenue = await _orderService.GetDailyRevenueAsync(date);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting daily revenue for {date}");
                return StatusCode(500, "Internal server error occurred");
            }
        }
    }
}
