using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Rest.Domain.Entities.Enums;

namespace Rest.Domain.Entities
{
    /// <summary>
    /// Represents an address associated with a user.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        [Key]
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        [Required]
        [StringLength(255)]
        public required string AddressLine1 { get; set; }

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
        public required string City { get; set; }

        [StringLength(100)]
        public string? Governorate { get; set; }

        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the address type (e.g., Home, Work, Other).
        /// </summary>
        [Required]
        public AddressType AddressType { get; set; }

        /// <summary>
        /// Is this address the default address for the user?
        /// </summary>
        public bool IsDefault { get; set; }

        [Required]
        public required string UserId { get; set; }

        // Navigation properties
        public virtual required User User { get; set; }

        public virtual required ICollection<Order> Orders { get; set; }
    }
}
