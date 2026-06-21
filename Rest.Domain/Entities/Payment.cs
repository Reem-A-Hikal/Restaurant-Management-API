using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Domain.Entities
{
    public class Payment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        /// <summary>
        /// Snapshot of the amount due at the moment this payment attempt
        /// was created — independent of later changes to Order.TotalAmount.
        /// </summary>
        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }
        public string? GatewayResponse { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual required Order Order { get; set; }

        #region State Machine
        private static readonly Dictionary<PaymentStatus, PaymentStatus[]> _allowedTransitions = new()
        {
            [PaymentStatus.Pending] = new[] { PaymentStatus.Completed, PaymentStatus.Failed },
            [PaymentStatus.Completed] = new[] { PaymentStatus.Refunded },
            [PaymentStatus.Failed] = Array.Empty<PaymentStatus>(),
            [PaymentStatus.Refunded] = Array.Empty<PaymentStatus>(),
        };

        public bool CanTransitionTo(PaymentStatus newStatus)
        {
            return _allowedTransitions.TryGetValue(Status, out var allowed)
                   && allowed.Contains(newStatus);
        }

        private void TransitionTo(PaymentStatus newStatus)
        {
            if (!CanTransitionTo(newStatus))
                throw new BusinessException(
                    $"Cannot transition payment from '{Status}' to '{newStatus}'.");

            Status = newStatus;
        }

        /// <summary>
        /// transactionId/gatewayResponse are optional here because Cash payments
        /// have no gateway. Enforcement of "Stripe must have a TransactionId"
        /// belongs in PaymentService, which knows the payment Method context.
        /// </summary>
        public void Complete(string? transactionId = null, string? gatewayResponse = null)
        {
            TransitionTo(PaymentStatus.Completed);
            TransactionId = transactionId;
            GatewayResponse = gatewayResponse;
            PaidAt = DateTime.UtcNow;
        }

        public void Fail(string? gatewayResponse = null)
        {
            TransitionTo(PaymentStatus.Failed);
            GatewayResponse = gatewayResponse;
        }

        public void Refund(string? gatewayResponse = null)
        {
            TransitionTo(PaymentStatus.Refunded);
            GatewayResponse = gatewayResponse;
        }
        #endregion
    }
}
