using Rest.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.UserDtos
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters")]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "User name must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9._@+-]+$", ErrorMessage = "User name contains invalid characters")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(11, ErrorMessage = "Phone number cannot exceed 11 characters")]
        public required string PhoneNumber { get; set; }

        [LocalImageUrl(ImageUrlRules.Users)]
        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "User role is required")]
        [AllowedValues("Admin", "Chef", "DeliveryPerson", "Customer",
            ErrorMessage = "Role must be Admin, Chef, DeliveryPerson, or Customer")]
        public required string UserRole { get; set; }

        [StringLength(100)]
        public string? Specialization { get; set; } // For Chef role

        [StringLength(20)]
        public string? VehicleNumber { get; set; }  // For DeliveryPerson role

        public bool IsAvailable { get; set; } = true; // For DeliveryPerson role
    }
}
