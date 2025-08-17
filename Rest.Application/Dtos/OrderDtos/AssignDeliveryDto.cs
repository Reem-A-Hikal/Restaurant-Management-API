using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.OrderDtos
{
    public class AssignDeliveryDto
    {
        [Required]
        public string DeliveryPersonId { get; set; }
    }
}
