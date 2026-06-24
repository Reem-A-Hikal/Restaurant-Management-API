using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;
using ValidationException = Rest.Domain.Exceptions.ValidationException;

namespace Rest.Domain.Entities
{
    /// <summary>
    /// Represents an order placed by a user.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        public required string OrderNumber { get; set; } // Auto-generated

        /// <summary>
        /// Gets or sets the date and time when the order was placed.
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date and time when the order is required.
        /// </summary>
        public DateTime? RequiredTime { get; set; }
        public DateTime? ConfirmationTime { get; set; }
        public DateTime? PreparationStartTime { get; set; }
        public DateTime? CancellationTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the order.
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.New; // New, Confirmed, Preparing, Ready, OutForDelivery, Delivered, Canceled

        /// <summary>
        /// Staff ID (Admin/Chef) who confirmed the order
        /// </summary>
        public string? ConfirmedById { get; set; }

        /// <summary>
        /// Items total before fees/discounts
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Gets or sets the delivery fee for the order.
        /// </summary>
        public decimal DeliveryFee { get; set; }

        /// <summary>
        /// Gets or sets the tax amount for the order.
        /// </summary>
        public decimal Tax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount for the order.
        /// </summary>
        public decimal Discount { get; set; } = 0.00m;
        /// <summary>
        /// Computed column in DB: SubTotal + DeliveryFee + Tax - Discount
        /// </summary>
        public decimal TotalAmount { get; private set; }

        /// <summary>
        /// Gets or sets the estimated delivery time in minutes.
        /// </summary>
        public int? EstimatedDeliveryTime { get; set; }

        /// <summary>
        /// Computed property — derived from Payments collection.
        /// A Payment record is never mutated after creation except via its own
        /// state machine (Complete/Fail/Refund); this property aggregates the
        /// latest meaningful state across all payment attempts for this order.
        /// </summary>
        public PaymentStatus PaymentStatus { get
            {
                if (!Payments.Any())
                    return PaymentStatus.Pending;

                if (Payments.Any(p => p.Status == PaymentStatus.Completed))
                    return PaymentStatus.Completed;

                if (Payments.Any(p => p.Status == PaymentStatus.Refunded))
                    return PaymentStatus.Refunded;

                if (Payments.All(p => p.Status == PaymentStatus.Failed))
                    return PaymentStatus.Failed;

                return PaymentStatus.Pending;
            }
        }
        public bool IsPaid => PaymentStatus == PaymentStatus.Completed;

        /// <summary>
        /// Notes written by the customer at order creation time.
        /// </summary>
        public string? CustomerNotes { get; set; }

        /// <summary>
        /// Notes written by Admin/Chef (confirmation notes, cancellation reason, etc.).
        /// </summary>
        public string? StaffNotes { get; set; }

        /// <summary>
        /// Gets or sets the source of the order.
        /// </summary>
        public OrderSource Source { get; set; } = OrderSource.Website;

        /// <summary>
        /// UserId of the customer who placed the order.
        /// </summary>
        // Foreign keys
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets the delivery address for the order.
        /// </summary>
        public int DeliveryAddressId { get; set; }


        // Navigation properties
        /// <summary>
        /// User who placed the order.
        /// </summary>
        public virtual required User User { get; set; }

        /// <summary>
        /// Gets or sets the delivery address for the order.
        /// </summary>
        public virtual required Address DeliveryAddress { get; set; }

        public virtual User? ConfirmedBy { get; set; }

        /// <summary>
        /// Gets or sets the collection of order details associated with the order.
        /// </summary>
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        /// <summary>
        /// One-to-Many: each delivery-person assignment attempt is a separate record
        /// (mirrors the Payment pattern — a cancelled assignment doesn't get reused,
        /// a new Delivery record is created instead).
        /// </summary>
        public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

        /// <summary>
        /// Gets or sets the review associated with the order.
        /// </summary>
        public virtual Review? Review { get; set; }

        #region Start Machine
        private static readonly Dictionary<OrderStatus, OrderStatus[]> _allowedTransitions = new()
        {
            [OrderStatus.New] = new[] {OrderStatus.Confirmed, OrderStatus.Canceled},
            [OrderStatus.Confirmed] = new[] {OrderStatus.Preparing, OrderStatus.Canceled},
            [OrderStatus.Preparing] = new[] { OrderStatus.Ready, OrderStatus.Canceled },
            [OrderStatus.Ready] = new[] { OrderStatus.OutForDelivery, OrderStatus.Canceled },
            [OrderStatus.OutForDelivery] = new[] { OrderStatus.Delivered },
            [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
            [OrderStatus.Canceled] = Array.Empty<OrderStatus>(),
        };

        public bool CanTransitionTo(OrderStatus newStatus)
        {
            return _allowedTransitions.TryGetValue(Status, out var allowed) 
                && allowed.Contains(newStatus);
        }

        /// <summary>
        /// General-purpose transition engine for statuses that carry no
        /// mandatory extra data. Confirm() and Cancel() use this internally
        /// too, then layer on their own required data.
        /// </summary>
        public void TransitionTo(OrderStatus newStatus)
        {
            if(!CanTransitionTo(newStatus))
                throw new BusinessException($"Cannot transition order from '{Status}' to '{newStatus}'.");

            Status = newStatus;

            switch (newStatus)
            {
                case OrderStatus.Preparing:
                    PreparationStartTime = DateTime.UtcNow;
                    break;
                case OrderStatus.Canceled:
                    CancellationTime = DateTime.UtcNow;
                    break;
            }
        }

        public void Confirm(string confirmedById, DateTime? requiredTime = null, string? staffNotes = null)
        {
            TransitionTo(OrderStatus.Confirmed);
            ConfirmedById = confirmedById;
            ConfirmationTime = DateTime.UtcNow;

            if (requiredTime.HasValue)
                RequiredTime = requiredTime.Value;

            if (!string.IsNullOrWhiteSpace(staffNotes))
                StaffNotes = staffNotes;
        }

        public void Cancel(string cancellationReason)
        {
            if (string.IsNullOrWhiteSpace(cancellationReason))
                throw new ValidationException("Cancellation reason is required.");

            TransitionTo(OrderStatus.Canceled);

            StaffNotes = string.IsNullOrWhiteSpace(StaffNotes)
                ? $"Cancellation Reason: {cancellationReason}"
                : $"{StaffNotes}\nCancellation Reason: {cancellationReason}";
        }
        #endregion

        #region Domain Methods
        /// <summary>
        /// Adds a product to the order, or merges quantity into an existing
        /// line item if the product is already present.
        /// </summary>
        public void AddItem(Product product, int quantity)
        {
            if (Status != OrderStatus.New)
                throw new BusinessException("Cannot modify items after the order has been confirmed.");

            if (quantity <= 0)
                throw new ValidationException("Quantity must be at least 1.");

            var existingDetail = OrderDetails.FirstOrDefault(od => od.ProductId == product.ProductId);
            if (existingDetail != null)
            {
                existingDetail.Quantity += quantity;
            }
            else
            {
                OrderDetails.Add(new OrderDetail
                {
                    Order = this,
                    Product = product,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    UnitPrice = product.GetDiscountedPrice()
                });
            }
            RecalculateSubTotal();
        }

        private void RecalculateSubTotal()
        {
            SubTotal = OrderDetails.Sum(od => od.UnitPrice * od.Quantity);
        }

        public void RemoveItem(int productId)
        {
            if (Status != OrderStatus.New)
                throw new BusinessException("Cannot modify items after the order has been confirmed.");

            var detail = OrderDetails.FirstOrDefault(od => od.ProductId == productId)
                ?? throw new NotFoundException("Product in order", productId);

            OrderDetails.Remove(detail);
            RecalculateSubTotal();
        }
        #endregion
    }
}
