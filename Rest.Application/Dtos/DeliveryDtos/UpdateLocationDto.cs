using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.DeliveryDtos
{
    public class UpdateLocationDto
    {
        [Required]
        [Range(-90, 90)]
        public decimal Latitude { get; set; }

        [Required]
        [Range(-180, 180)]
        public decimal Longitude { get; set; }
    }
}
