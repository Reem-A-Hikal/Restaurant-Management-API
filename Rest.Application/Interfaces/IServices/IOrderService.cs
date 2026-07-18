using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Application.Dtos.OrderDtos;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();

        Task<OrderDto> CreateOrderAsync(string userId,CreateOrderDto orderDto);

        Task<OrderDto> ConfirmOrderAsync(int orderId, string confirmedBy, ConfirmOrderDto confirmDto);
        Task<OrderDto> CancelOrderAsync(int orderId, string cancellationReason, bool isCustomer = false);
        Task<OrderDto> MarkAsPreparingAsync(int orderId);
        Task<OrderDto> MarkAsPreparedAsync(int orderId);

        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);

        /// <summary>
        /// Returns the order statuses the given role is allowed to filter by.
        /// Frontend uses this to build the status filter dropdown dynamically -
        /// no status list is ever hardcoded on the client.
        /// </summary>
        IEnumerable<OrderStatus> GetAllowedStatusesForRole(string role);

        // <summary>
        /// Returns orders scoped to what the given role is allowed to see.
        /// Admin sees everything; Chef sees only Confirmed/Preparing/Ready.
        /// </summary>
        Task<IEnumerable<OrderDto>> GetOrdersVisibleToRoleAsync(string role);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<OrderDto>> GetKitchenQueueAsync();

        Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetDailyRevenueAsync(DateTime date);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);

        Task<OrderDto> AddOrderItemAsync(int orderId, CreateOrderDetailDto itemDto);
        Task<OrderDto> RemoveOrderItemAsync(int orderId, int productId);
    }
}
