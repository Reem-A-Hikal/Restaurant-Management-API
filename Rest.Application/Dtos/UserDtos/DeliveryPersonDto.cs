namespace Rest.Application.Dtos.UserDtos
{
    public class DeliveryPersonDto : UserDto
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }

    }
}
