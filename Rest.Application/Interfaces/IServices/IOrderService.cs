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
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<OrderDto>> GetKitchenQueueAsync();

        Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetDailyRevenueAsync(DateTime date);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);

        Task<OrderDto> AddOrderItemAsync(int orderId, CreateOrderDetailDto itemDto);
        Task<OrderDto> RemoveOrderItemAsync(int orderId, int productId);
    }
}
