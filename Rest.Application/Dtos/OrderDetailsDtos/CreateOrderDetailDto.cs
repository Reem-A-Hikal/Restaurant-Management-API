using Rest.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.OrderDetailsDtos
{
    public class CreateOrderDetailDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, Order.MaxQuantityPerItem, ErrorMessage = "Quantity must be between 1 and 10")]
        public int Quantity { get; set; }
    }
}
