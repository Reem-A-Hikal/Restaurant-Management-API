using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IOrderRepository :IRepository<Order>
    {
        ///////////////////////
        /// <summary>
        /// Order + OrderDetails + Product. For operations that need to read or
        /// recalculate the order's line items (e.g. AddOrderItem, RemoveOrderItem).
        /// </summary>
        Task<Order?> GetByIdWithDetailsAsync(int id);

        /// <summary>
        /// Full graph — every navigation populated (User, DeliveryAddress,
        /// ConfirmedBy, OrderDetails.Product, Payments, Deliveries, Review).
        /// For displaying a complete order (invoice view, admin detail page).
        /// </summary>
        Task<Order?> GetByIdFullAsync(int id);

        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetKitchenQueueAsync();
        Task<decimal> GetDailyRevenueAsync(DateTime date);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);
    }
}
