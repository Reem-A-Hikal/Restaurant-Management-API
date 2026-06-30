using Rest.Application.Dtos.DeliveryDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IDeliveryService
    {
        /// <summary>
        /// Assigns a delivery person to an order, creating a new Delivery with status Assigned.
        /// Throws if the order already has an active delivery.
        /// </summary>
        Task<DeliveryDto> AssignDeliveryAsync(int orderId, AssignDeliveryDto dto);

        /// <summary>
        /// Marks the specified delivery as PickedUp (only if its current status is Assigned).
        /// Verifies that the caller is the assigned delivery person.
        /// </summary>
        Task<DeliveryDto> MarkAsPickedUpAsync(int deliveryId);

        /// <summary>
        /// Marks the specified delivery as Delivered (only if its current status is PickedUp).
        /// </summary>
        Task<DeliveryDto> MarkAsDeliveredAsync(int deliveryId);

        /// <summary>
        /// Cancels the specified delivery with a mandatory reason.
        /// </summary>
        Task<DeliveryDto> CancelDeliveryAsync(int deliveryId, string reason);

        /// <summary>
        /// Returns the active delivery (Assigned or PickedUp) for a given order, or null.
        /// </summary>
        Task<DeliveryDto?> GetActiveDeliveryForOrderAsync(int orderId);

        /// <summary>
        /// Returns the full history of deliveries for an order.
        /// </summary>
        Task<IEnumerable<DeliveryDto>> GetDeliveryHistoryAsync(int orderId);

        Task<DeliveryDto?> GetDeliveryByIdAsync(int deliveryId);

        /// <summary>
        /// Returns all active deliveries assigned to a specific person.
        /// </summary>
        Task<IEnumerable<DeliveryDto>> GetMyDeliveriesAsync(string deliveryPersonId);

        /// <summary>
        /// Checks if a delivery person currently has any active delivery.
        /// </summary>
        Task<bool> HasActiveDeliveryAsync(string deliveryPersonId);

        /// <summary>
        /// Returns all active deliveries in the system (for dashboards).
        /// </summary>
        Task<IEnumerable<DeliveryDto>> GetAllActiveDeliveriesAsync();

        Task<DeliveryDto> UpdateLocationAsync(int deliveryId, string deliveryPersonId, UpdateLocationDto dto);
    }
}
