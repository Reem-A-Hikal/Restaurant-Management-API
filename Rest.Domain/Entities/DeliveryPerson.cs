using Rest.Domain.Exceptions;

namespace Rest.Domain.Entities
{
    public class DeliveryPerson :User
    {
        public string VehicleNumber { get; private set; } = null!;

        public bool? IsAvailable { get; private set; } = true;

        public virtual ICollection<Delivery> Deliveries { get; set; } = [];

        public static DeliveryPerson Create(
            string email, string userName, string fullName,
            string? phoneNumber, string? profileImageUrl, string vehicleNumber)
        {
            var dp = InitializeBase(new DeliveryPerson(), email, userName, fullName, phoneNumber, profileImageUrl);
            dp.SetVehicleNumber(vehicleNumber);
            dp.IsAvailable = true;
            return dp;
        }

        public void SetVehicleNumber(string vehicleNumber)
        {
            if (string.IsNullOrWhiteSpace(vehicleNumber))
                throw new ValidationException("Vehicle number is required for a Delivery Person.");

            VehicleNumber = vehicleNumber;
        }

        public void MarkAvailable() => IsAvailable = true;
        public void MarkBusy() => IsAvailable = false;
        public void SetAvailability(bool isAvailable) => IsAvailable = isAvailable;
    }
}
