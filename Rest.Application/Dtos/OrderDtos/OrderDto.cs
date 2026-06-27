using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.OrderDtos
{
    public class OrderDto
    {
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the order was placed.
        /// </summary>
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the status of the order.
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.New;
        public string StatusDisplay { get; set; }
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
        /// Gets or sets the total amount for the order.
        /// </summary>
        public decimal TotalAmount { get; set; } // Total amount including items, delivery fee, tax, and discount

        /// <summary>
        /// Gets or sets the payment status of the order.
        /// </summary>
        public PaymentStatus PaymentStatus { get; set; }
        public bool IsPaid { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerId { get; set; }

        public string? CustomerAddress { get; set; }

        public string? CustomerNotes { get; set; }
        public string? StaffNotes { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }
}
