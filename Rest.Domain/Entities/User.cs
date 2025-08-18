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


        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int UserId { get; set; }

        //[Required(ErrorMessage = "Username is required")]
        //[StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        //[Column(TypeName = "varchar(50)")]
        //[RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Invalid username format")]
        //public string Username { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        //[Column(TypeName = "varchar(100)")]
        //[DataType(DataType.Password)]
        //[JsonIgnore]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        //[EmailAddress(ErrorMessage = "Invalid email format")]
        //[RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        //[Column(TypeName = "varchar(100)")]
        //public string Email { get; set; }

    }
}
