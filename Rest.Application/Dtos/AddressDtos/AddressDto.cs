using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.AddressDtos
{
    public class AddressDto
    {
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        [Required]
        [StringLength(255)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Gets or sets the second line of the address (optional).
        /// </summary>

        [StringLength(255)]
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Gets or sets city name.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets latitude of the address.
        /// </summary>
        /// [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets longitude of the address.
        /// </summary>
        /// [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the address type (e.g., Home, Work, Other).
        /// </summary>
        [Required]
        [Column(TypeName = "nvarchar(20)")]
        public AddressType AddressType { get; set; } // Home, Work, Other

        /// <summary>
        /// Is this address the default address for the user?
        /// </summary>

        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets UserId of the user associated with this address..
        /// </summary>
        // Foreign key
        [Required]
        public string UserId { get; set; }
    }
}
