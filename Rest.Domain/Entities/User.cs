using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rest.Domain.Entities
{
    public class User : IdentityUser
    {

        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        [Column(TypeName = "nvarchar(100)")]
        public string FullName { get; set; }

        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [InverseProperty("User")]
        public virtual ICollection<Order> CustomerOrders { get; set; } = [];

        [InverseProperty("DeliveryPerson")]
        public virtual ICollection<Order> DeliveryOrders { get; set; } = [];
        public virtual ICollection<Address> Addresses { get; set; } = [];
        public virtual ICollection<Delivery> Deliveries { get; set; } = [];
        public virtual ICollection<Review> Reviews { get; set; } = [];

    }
}
