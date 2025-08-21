using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rest.Application.Dtos.UserDtos
{
    /// <summary>
    /// User profile update data transfer object
    /// </summary>
    public class UpdateProfileDto
    {
        ///// <summary>
        ///// Unique identifier for the user
        /////</summary>
        /////
        //[Required]
        //public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User's full name
        /// </summary>
        /// <example>John Doe</example>
        /// 
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string? FullName { get; set; }

        /// <summary>
        /// User's phone number
        /// </summary>
        /// <example>01234567890</example>
        [Phone(ErrorMessage = "Phone number is not valid")]
        [StringLength(11, ErrorMessage = "Phone number cannot exceed 11 characters")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        ///  profile image
        /// </summary>
        /// <example>user.jpg</example>
        public string? ProfileImageUrl { get; set; }

        /// <summary>
        /// Chef's specialization (visible only for Chef role)
        /// </summary>
        /// <example>Italian Cuisine</example>
        [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Specialization { get; set; } // Chef

        /// <summary>
        /// Delivery person's vehicle number (visible only for DeliveryPerson role)
        /// </summary>
        /// <example>ABC-1234</example>
        [StringLength(20, ErrorMessage = "Vehicle number cannot exceed 20 characters")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? VehicleNumber { get; set; } // Driver
        /// <summary>
        /// Indicates if the delivery person is currently available (visible only for DeliveryPerson role)
        /// </summary>
        public bool? IsAvailable { get; set; }

    }
}
