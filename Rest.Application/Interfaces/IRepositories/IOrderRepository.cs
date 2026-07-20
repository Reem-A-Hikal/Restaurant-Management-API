using Rest.Application.Dtos.DashboardDtos;
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

        /// <summary>
        /// Gets orders matching any of the given statuses. Used for
        /// role-scoped views (e.g. Chef sees Confirmed/Preparing/Ready only)
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByStatusesAsync(IEnumerable<OrderStatus> statuses);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetKitchenQueueAsync();
        Task<decimal> GetDailyRevenueAsync(DateTime date);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);

        /// <summary>
        /// Counts of orders grouped by status, in one query — used for the dashboard
        /// status-breakdown donut chart instead of calling GetOrderCountByStatusAsync per status.
        /// </summary>
        Task<Dictionary<OrderStatus, int>> GetOrderCountsByStatusAsync();

        /// <summary>
        /// Daily revenue for the last N days (including zero-revenue days), ordered
        /// chronologically. Used for the dashboard revenue trend line chart.
        /// </summary>
        Task<List<RevenueTrendPointDto>> GetRevenueTrendAsync(int days);

        Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count);
    }
}
