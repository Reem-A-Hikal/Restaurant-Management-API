using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rest.Domain.Entities
{
    public class DeliveryPerson :User
    {
        /// <summary>
        /// Delivery person's vehicle number (visible only for DeliveryPerson role)
        /// </summary>
        /// <example>ABC-1234</example>
        [Required]
        [StringLength(20, ErrorMessage = "Vehicle number cannot exceed 20 characters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string VehicleNumber { get; set; }

        /// <summary>
        /// Indicates if the delivery person is currently available (visible only for DeliveryPerson role)
        /// </summary>
        /// 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsAvailable { get; set; }
    }
}
