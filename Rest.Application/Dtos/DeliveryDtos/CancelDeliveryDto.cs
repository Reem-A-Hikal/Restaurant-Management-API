using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.DeliveryDtos
{
    public class CancelDeliveryDto
    {
        [Required]
        [StringLength(500)]
        public string Reason { get; set; }
    }
}
