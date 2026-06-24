using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        /// <summary>
        /// Gets all order details for a given order, including Product and Order navigation.
        /// Used for displaying order contents (invoices, order summaries).
        /// </summary>
        Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// Checks if a specific product already exists as a line item in the given order.
        /// Used by OrderService to decide whether to merge quantities or add a new line.
        /// </summary>
        Task<OrderDetail?> GetByOrderIdAndProductIdAsync(int orderId, int productId);
    }
}
