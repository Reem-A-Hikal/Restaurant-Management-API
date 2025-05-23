using Rest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rest.API.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public int DeliveryAddressId { get; set; }

        /// <summary>
        /// Gets or sets the discount amount for the order.
        /// </summary>
        public decimal Discount { get; set; } = 0.00m;
        public decimal Tax { get; set; } // Tax per item
        public decimal DeliveryFee { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Order must have at least one item")]
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
    }
}
