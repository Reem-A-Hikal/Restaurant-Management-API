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
        /// Max quantity of a single product in one order.
        /// Keep CreateOrderDetailDto's [Range] and the UI in sync with this value.
        /// </summary>
        public const int MaxQuantityPerItem = 10;

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
        public OrderStatus Status { get; set; } = OrderStatus.New; // New, Confirmed, Preparing, Ready, OutForDelivery, Delivered, Cancelled

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
        public PaymentStatus PaymentStatus
        {
            get
            {
                if (!Payments.Any())
                    return PaymentStatus.Pending;

                if (Payments.Any(p => p.Status == PaymentStatus.Refunded))
                    return PaymentStatus.Refunded;

                if (Payments.Any(p => p.Status == PaymentStatus.Completed))
                    return PaymentStatus.Completed;

                if (Payments.All(p => p.Status == PaymentStatus.Failed))
                    return PaymentStatus.Failed;

                return PaymentStatus.Pending;
            }
        }
        public bool IsPaid => PaymentStatus == PaymentStatus.Completed;

        /// <summary>
        /// Notes written by the customer at order creation time
        /// (and the customer's cancellation reason, if they cancel).
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

        // Foreign keys
        /// <summary>
        /// UserId of the customer who placed the order.
        /// </summary>
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

        #region State Machine
        private static readonly Dictionary<OrderStatus, OrderStatus[]> _allowedTransitions = new()
        {
            [OrderStatus.New] = new[] { OrderStatus.Confirmed, OrderStatus.Cancelled },
            [OrderStatus.Confirmed] = new[] { OrderStatus.Preparing, OrderStatus.Cancelled },
            [OrderStatus.Preparing] = new[] { OrderStatus.Ready, OrderStatus.Cancelled },
            [OrderStatus.Ready] = new[] { OrderStatus.OutForDelivery, OrderStatus.Cancelled },
            [OrderStatus.OutForDelivery] = new[] { OrderStatus.Delivered },
            [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
            [OrderStatus.Cancelled] = Array.Empty<OrderStatus>(),
        };

        /// <summary>
        /// Statuses from which a customer may cancel on their own.
        /// From Preparing onward, cancelling goes through the restaurant.
        /// </summary>
        private static readonly OrderStatus[] _customerCancellable =
        {
            OrderStatus.New,
            OrderStatus.Confirmed,
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
        /// Do not use it to cancel: use Cancel(), which enforces the reason
        /// and the customer rule.
        /// </summary>
        public void TransitionTo(OrderStatus newStatus)
        {
            if (!CanTransitionTo(newStatus))
                throw new BusinessException($"Cannot transition order from '{Status}' to '{newStatus}'.");

            Status = newStatus;

            switch (newStatus)
            {
                case OrderStatus.Preparing:
                    PreparationStartTime = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
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

        /// <summary>
        /// Cancels the order. When <paramref name="isCustomer"/> is true the customer
        /// rule applies (New/Confirmed only) and the reason is stored in CustomerNotes;
        /// otherwise it is stored in StaffNotes.
        /// </summary>
        public void Cancel(string cancellationReason, bool isCustomer = false)
        {
            if (string.IsNullOrWhiteSpace(cancellationReason))
                throw new ValidationException("Cancellation reason is required.");

            if (isCustomer && !_customerCancellable.Contains(Status))
                throw new BusinessException(
                    "This order can no longer be cancelled online. Please contact the restaurant.",
                    ErrorCodes.OrderCancelNotAllowed);

            TransitionTo(OrderStatus.Cancelled);

            if (isCustomer)
                CustomerNotes = string.IsNullOrWhiteSpace(CustomerNotes)
                    ? $"Cancellation Reason: {cancellationReason}"
                    : $"{CustomerNotes}\nCancellation Reason: {cancellationReason}";
            else
                StaffNotes = string.IsNullOrWhiteSpace(StaffNotes)
                    ? $"Cancellation Reason: {cancellationReason}"
                    : $"{StaffNotes}\nCancellation Reason: {cancellationReason}";
        }

        public void RevertToReadyAfterDeliveryCancel()
        {
            if (Status != OrderStatus.OutForDelivery)
                throw new BusinessException("Can only revert to Ready from OutForDelivery.");
            Status = OrderStatus.Ready;
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

            if (quantity < 1 || quantity > MaxQuantityPerItem)
                throw new ValidationException($"Quantity must be between 1 and {MaxQuantityPerItem}.");

            var existingDetail = OrderDetails.FirstOrDefault(od => od.ProductId == product.ProductId);
            if (existingDetail != null)
            {
                if (existingDetail.Quantity + quantity > MaxQuantityPerItem)
                    throw new ValidationException($"Maximum {MaxQuantityPerItem} per item.");

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

        /// <summary>
        /// Sets the order charges. Call it after the items are added so the
        /// discount can be validated against the subtotal.
        /// </summary>
        public void ApplyCharges(decimal deliveryFee, decimal tax = 0m, decimal discount = 0m)
        {
            if (Status != OrderStatus.New)
                throw new BusinessException("Charges can only be changed while the order is New.");

            if (deliveryFee < 0 || tax < 0 || discount < 0)
                throw new ValidationException("Charges cannot be negative.");

            if (discount > SubTotal)
                throw new ValidationException("Discount cannot exceed the order subtotal.");

            DeliveryFee = deliveryFee;
            Tax = tax;
            Discount = discount;
        }
        #endregion
    }
}
