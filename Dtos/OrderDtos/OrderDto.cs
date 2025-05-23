using Rest.API.Dtos.AddressDtos;
using Rest.API.Dtos.OrderDetailsDtos;
using Rest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rest.API.Dtos.OrderDtos
{
    public class OrderDto
    {
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
        /// Gets or sets the status of the order.
        /// </summary>
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.New; // New, Confirmed, Preparing, Ready, OutForDelivery, Delivered, Canceled
        public string StatusDisplay { get; set; }
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
        /// Gets or sets the payment status of the order.
        /// </summary>
        [StringLength(20)]
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Failed

        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

        public string? CustomerAddress { get; set; }

        /// <summary>
        /// Gets or sets the delivery person assigned to the order.
        /// </summary>
        public string? DeliveryPersonName { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
