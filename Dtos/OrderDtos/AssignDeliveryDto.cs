using System.ComponentModel.DataAnnotations;

namespace Rest.API.Dtos.OrderDtos
{
    public class AssignDeliveryDto
    {
        [Required]
        public string DeliveryPersonId { get; set; }
    }
}
