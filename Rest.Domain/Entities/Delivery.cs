using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Domain.Entities
{
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public DateTime StatusChangeTime { get; set; } = DateTime.UtcNow;
        public DeliveryStatus Status { get; private set; } = DeliveryStatus.Assigned;

        /// <summary>
        /// Captured once, at creation — NOT overwritten by TransitionTo().
        /// </summary>
        public DateTime AssignedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? DeliveryStartTime { get; private set; }
        public DateTime? DeliveryEndTime { get; private set; }
        public DateTime? CancelledAt { get; private set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
        public string? Notes { get; set; }
        public string DeliveryPersonId { get; set; }
        public int OrderId { get; set; }

        // Navigation properties
        public virtual DeliveryPerson? DeliveryPerson { get; set; }
        public virtual Order? Order { get; set; }

        #region State Machine
        private static readonly Dictionary<DeliveryStatus, DeliveryStatus[]> _allowedTransitions = new()
        {
            [DeliveryStatus.Assigned] = new[] { DeliveryStatus.PickedUp, DeliveryStatus.Cancelled },
            [DeliveryStatus.PickedUp] = new[] { DeliveryStatus.Delivered, DeliveryStatus.Cancelled },
            [DeliveryStatus.Delivered] = Array.Empty<DeliveryStatus>(),
            [DeliveryStatus.Cancelled] = Array.Empty<DeliveryStatus>(),
        };

        public bool CanTransitionTo(DeliveryStatus newStatus)
        {
            return _allowedTransitions.TryGetValue(Status, out var allowed)
                   && allowed.Contains(newStatus);
        }

        private void TransitionTo(DeliveryStatus newStatus)
        {
            if (!CanTransitionTo(newStatus))
                throw new BusinessException(
                    $"Cannot transition delivery from '{Status}' to '{newStatus}'.");

            Status = newStatus;
            StatusChangeTime = DateTime.UtcNow;
        }

        public void MarkAsPickedUp()
        {
            TransitionTo(DeliveryStatus.PickedUp);
            DeliveryStartTime = DateTime.UtcNow;
        }

        public void MarkAsDelivered()
        {
            TransitionTo(DeliveryStatus.Delivered);
            DeliveryEndTime = DateTime.UtcNow;
        }

        public void Cancel(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ValidationException("Cancellation reason is required.");

            TransitionTo(DeliveryStatus.Cancelled);

            Notes = string.IsNullOrWhiteSpace(Notes)
                ? $"Cancellation Reason: {reason}"
                : $"{Notes}\nCancellation Reason: {reason}";
        }
        #endregion

        #region Domain Method
        public void UpdateLocation(decimal latitude,  decimal longitude)
        {
            if (Status != DeliveryStatus.Assigned && Status != DeliveryStatus.PickedUp)
                throw new BusinessException("Cannot update location for a completed or cancelled delivery");

            Latitude = latitude;
            Longitude = longitude;
        }
        #endregion
    }
}
