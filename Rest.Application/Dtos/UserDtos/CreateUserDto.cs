using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Rest.Application.Dtos.UserDtos
{
    public class CreateUserDto
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ProfileImageUrl { get; set; }

        [Required]
        public string Password { get; set; }
        public string UserRole { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Specialization { get; set; } // For Chef role

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? VehicleNumber { get; set; } // For DeliveryPerson role
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool IsAvailable { get; set; } = true; // For DeliveryPerson role

        //// Optional: Add address at creation
        //public List<CreateAddressDto> Addresses { get; set; } = new();
    }
}
