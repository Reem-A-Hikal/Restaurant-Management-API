using Rest.Application.Dtos.OrderDetailsDtos;
using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        [Required]
        public int DeliveryAddressId { get; set; }

        [StringLength(1000)]
        public string? CustomerNotes { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Order must have at least one item")]
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
    }
}
