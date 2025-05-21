using Rest.API.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rest.API.Dtos.AddressDtos
{
    /// <summary>
    /// 
    /// </summary>
    public class AddressUpdateDto
    {
        [StringLength(255, ErrorMessage = "Address line 1 cannot exceed 255 characters")]
        public string? AddressLine1 { get; set; }

        [StringLength(255, ErrorMessage = "Address line 2 cannot exceed 255 characters")]
        public string? AddressLine2 { get; set; }

        [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
        public string? City { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }

        public AddressType? AddressType { get; set; }

        public bool? IsDefault { get; set; }
    }
}
