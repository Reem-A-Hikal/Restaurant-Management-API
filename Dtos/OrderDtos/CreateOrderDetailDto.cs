using System.ComponentModel.DataAnnotations;

namespace Rest.API.Dtos.OrderDtos
{
    public class CreateOrderDetailDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; } // Price per item
       
    }
}
