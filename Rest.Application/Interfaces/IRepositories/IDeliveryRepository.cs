using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IDeliveryRepository : IRepository<Delivery>
    {
        /// <summary>
        /// Returns the currently active delivery attempt for an order
        /// (status Assigned or PickedUp) — null if none, or if the order
        /// was delivered/cancelled with no active attempt in progress.
        /// </summary>
        Task<Delivery?> GetActiveDeliveryByOrderIdAsync(int orderId);

        /// <summary>
        /// Full delivery attempt history for an order (Assigned, Cancelled,
        /// Assigned again, Delivered, ...).
        /// </summary>
        Task<IEnumerable<Delivery>> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// Active assignments currently on a given delivery person's plate
        /// (status Assigned or PickedUp). Used to answer "what am I delivering right now?"
        /// </summary>
        Task<IEnumerable<Delivery>> GetActiveDeliveriesByPersonIdAsync(string deliveryPersonId);
    }
}