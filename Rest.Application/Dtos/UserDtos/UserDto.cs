using Rest.Application.Dtos.AddressDtos;
using System.Text.Json.Serialization;

namespace Rest.Application.Dtos.UserDtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Specialization { get; set; } // For Chef role

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? VehicleNumber { get; set; } // For DeliveryPerson role

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsAvailable { get; set; } = true; // For DeliveryPerson role

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<AddressDto> Addresses { get; set; } = [];

    }
}
