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
        public required string UserId { get; set; }

        /// <summary>
        /// Gets or sets User associated with this address.
        /// </summary>

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual required User User { get; set; }

        /// <summary>
        /// Gets or sets the collection of orders associated with this address.
        /// </summary>

        public virtual required ICollection<Order> Orders { get; set; }
    }
}
