using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.UserDtos
{
    public class CreateUserDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public required string Password { get; set; }

        [Required(ErrorMessage ="UserName role is required")]
        public required string UserRole { get; set; }

        public string? Specialization { get; set; } // For Chef role

        public string? VehicleNumber { get; set; } // For DeliveryPerson role
        public bool IsAvailable { get; set; } = true; // For DeliveryPerson role
    }
}
