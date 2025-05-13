using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Rest.API.Models.Enums;

namespace Rest.API.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } // Auto-generated

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? RequiredTime { get; set; }

        [Required]
        public OrderStatus Status { get; set; } // New, Confirmed, Preparing, Ready, OutForDelivery, Delivered, Canceled

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; } // Total amount including items, delivery fee, tax, and discount

        [Range(0, int.MaxValue, ErrorMessage = "Estimated delivery time must be positive")]
        public int? EstimatedDeliveryTime { get; set; } // In minutes

        public PaymentMethod? PaymentMethod { get; set; } // Stripe, Cash

        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DeliveryFee { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Tax { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Discount { get; set; } = 0.00m;

        [StringLength(1000)]
        public string? Notes { get; set; }

        public OrderSource Source { get; set; } // Website, Phone, ThirdParty

        // Foreign keys
        [Required]
        public string UserId { get; set; }

        [Required]
        public int DeliveryAddressId { get; set; }

        public string? DeliveryPersonId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        [InverseProperty("CustomerOrders")]
        public virtual User User { get; set; }

        [ForeignKey("DeliveryAddressId")]
        public virtual Address DeliveryAddress { get; set; }

        [ForeignKey("DeliveryPersonId")]
        [InverseProperty("DeliveryOrders")]
        public virtual User? DeliveryPerson { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public virtual Delivery? Delivery { get; set; }

        public virtual Review? Review { get; set; }
    }
}
