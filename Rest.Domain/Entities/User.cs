using Microsoft.AspNetCore.Identity;
using Rest.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rest.Domain.Entities
{
    public class User : IdentityUser
    {

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public required string FullName { get; set; }

        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public DateTime JoinDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        public bool IsActive => Status == UserStatus.Active;
        public bool IsDeleted => Status == UserStatus.Deleted;

        // Navigation properties
        [InverseProperty("User")]
        public virtual ICollection<Order> CustomerOrders { get; set; } = [];
        public virtual ICollection<Address> Addresses { get; set; } = [];
        public virtual ICollection<Review> Reviews { get; set; } = [];
    }
}
