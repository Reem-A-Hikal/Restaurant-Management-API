using Rest.API.Dtos.OrderDtos;
using Rest.API.Models.Enums;

namespace Rest.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> CreateOrderAsync(CreateOrderDto orderDto);
        Task UpdateOrderAsync(int id, UpdateOrderDto orderDto);
        Task DeleteOrderAsync(int id);


        Task<OrderDto> ConfirmOrderAsync(int orderId, string confirmedBy, ConfirmOrderDto confirmDto);
        Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<OrderDto> CancelOrderAsync(int orderId, string cancellationReason);


        Task<OrderDto> AssignDeliveryPersonAsync(int orderId, string deliveryPersonId);
        Task<OrderDto> MarkAsDeliveredAsync(int orderId);


        Task<OrderDto> UpdatePaymentStatusAsync(int orderId, string newStatus);
        Task ProcessPaymentAsync(int orderId, PaymentMethod paymentMethod);


        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId);
        //Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<OrderDto>> GetPendingDeliveryOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByDeliveryPersonAsync(string deliveryPersonId);

        Task<decimal> GetDailyRevenueAsync(DateTime date);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);


        Task<OrderDto> AddOrderItemAsync(int orderId, CreateOrderDetailDto itemDto);
        Task<OrderDto> RemoveOrderItemAsync(int orderId, int productId);


        Task<IEnumerable<OrderDto>> GetKitchenQueueAsync();
        Task<OrderDto> MarkAsPreparedAsync(int orderId);
    }
}
