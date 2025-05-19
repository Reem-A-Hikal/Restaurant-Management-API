using System.Text.Json.Serialization;

namespace Rest.API.Dtos.UserDtos
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();

        [JsonIgnore]
        public string? Specialization { get; set; } // Chef

        [JsonIgnore]
        public string? VehicleNumber { get; set; } // Driver

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? AdditionalInfo => GetAdditionalData();

        private object? GetAdditionalData()
        {
            if (Roles.Contains("Chef"))
            {
                return new { Specialization };
            }
            if (Roles.Contains("DeliveryPerson"))
            {
                return new { VehicleNumber };
            }
            return null;
        }

        public void SetSpecialization(string? value) => Specialization = value;
        public void SetVehicleNumber(string? value) => VehicleNumber = value;
    }
}
