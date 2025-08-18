using System.ComponentModel.DataAnnotations;

namespace Rest.Domain.Entities
{
    public class DeliveryPerson :User
    {
        [Required]
        public string VehicleNumber { get; set; }
    }
}
