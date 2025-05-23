using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Rest.API.Models.Enums;

namespace Rest.API.Models
{
    /// <summary>
    /// Represents an order placed by a user.
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } // Auto-generated

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
        public DateTime? DeliveryStartTime { get; set; }
        public DateTime? DeliveryEndTime { get; set; }
        public DateTime? CancellationTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the order.
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.New; // New, Confirmed, Preparing, Ready, OutForDelivery, Delivered, Canceled

        /// <summary>
        /// Staff ID who confirmed the order
        /// </summary>
        public string? ConfirmedBy { get; set; } // Staff ID who confirmed the order

        /// <summary>
        /// Items total before fees/discounts
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Gets or sets the delivery fee for the order.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DeliveryFee { get; set; }

        /// <summary>
        /// Gets or sets the tax amount for the order.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Tax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount for the order.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Discount { get; set; } = 0.00m;
        /// <summary>
        /// Gets or sets the total amount for the order.
        /// </summary>
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; } // Total amount including items, delivery fee, tax, and discount

        /// <summary>
        /// Gets or sets the estimated delivery time in minutes.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "Estimated delivery time must be positive")]
        public int? EstimatedDeliveryTime { get; set; } // In minutes

        /// <summary>
        /// Gets or sets the payment method used for the order.
        /// </summary>
        public PaymentMethod? PaymentMethod { get; set; } // Stripe, Cash

        /// <summary>
        /// Gets or sets the payment status of the order.
        /// </summary>
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed

        [StringLength(50)]
        public string? TransactionId { get; set; }
        /// <summary>
        /// Gets or sets the special instructions or notes for the order.
        /// </summary>
        [StringLength(1000)]
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets the source of the order.
        /// </summary>
        public OrderSource Source { get; set; } // Website, Phone, ThirdParty

        /// <summary>
        /// UserId of the customer who placed the order.
        /// </summary>
        // Foreign keys
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the delivery address for the order.
        /// </summary>
        [Required]
        public int DeliveryAddressId { get; set; }

        /// <summary>
        /// Gets or sets the delivery person assigned to the order.
        /// </summary>
        public string? DeliveryPersonId { get; set; }

        // Navigation properties

        /// <summary>
        /// User who placed the order.
        /// </summary>
        [ForeignKey("UserId")]
        [InverseProperty("CustomerOrders")]
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the delivery address for the order.
        /// </summary>
        [ForeignKey("DeliveryAddressId")]
        public virtual Address DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets the delivery person assigned to the order.
        /// </summary>
        [ForeignKey("DeliveryPersonId")]
        [InverseProperty("DeliveryOrders")]
        public virtual User? DeliveryPerson { get; set; }

        /// <summary>
        /// Gets or sets the collection of order details associated with the order.
        /// </summary>
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        /// <summary>
        /// Gets or sets the delivery information for the order.
        /// </summary>
        public virtual Delivery? Delivery { get; set; }

        /// <summary>
        /// Gets or sets the review associated with the order.
        /// </summary>
        public virtual Review? Review { get; set; }
    }
}
