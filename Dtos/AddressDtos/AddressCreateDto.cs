using Rest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rest.API.Dtos.AddressDtos
{
    /// <summary>
    /// Data Transfer Object for creating a new address.
    /// </summary>
    public class AddressCreateDto
    {
        /// <summary>
        /// Unique identifier for the address.
        /// </summary>
        [JsonIgnore]
        public int AddressId { get; set; }

        /// <summary>
        /// Street address line 1.
        /// </summary>
        [Required(ErrorMessage = "Address line 1 is required")]
        [StringLength(255, ErrorMessage = "Address line 1 cannot exceed 255 characters")]
        
        public string AddressLine1 { get; set; }
        /// <summary>
        /// Street address line 2 (optional).
        /// </summary>

        [StringLength(255, ErrorMessage = "Address line 2 cannot exceed 255 characters")]
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// City name.
        /// </summary>
        [Required(ErrorMessage = "City is required")]
        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string City { get; set; }

        /// <summary>
        /// Latitude of the address location (optional).
        /// </summary>

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]

        /// <summary>
        /// Longitude of the address location (optional).
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Address type (e.g., Home, Work, Other).
        /// </summary>

        [Required(ErrorMessage = "Address type is required")]
        public AddressType? AddressType { get; set; } // "Home", "Work", "Other"

        /// <summary>
        /// Is this address the default address for the user?
        /// </summary>

        public bool IsDefault { get; set; } = false;
    }
}
