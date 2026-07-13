namespace Rest.Application.Dtos.UserDtos
{
    public class CreateUserDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string Password { get; set; }
        public string UserRole { get; set; }

        public string? Specialization { get; set; } // For Chef role

        public string? VehicleNumber { get; set; } // For DeliveryPerson role
        public bool IsAvailable { get; set; } = true; // For DeliveryPerson role
    }
}
