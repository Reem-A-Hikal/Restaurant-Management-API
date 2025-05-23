using Rest.API.Models;
using Rest.API.Models.Enums;

namespace Rest.API.Repositories.Interfaces
{
    public interface IOrderRepository :IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetPendingDeliveryOrdersAsync();
        Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
        Task<decimal> GetDailyRevenueAsync(DateTime date);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);

        Task AssignDeliveryPersonAsync(int orderId, string deliveryPersonId);
        Task<IEnumerable<Order>> GetOrdersByDeliveryPersonAsync(string deliveryPersonId);
    }
}
