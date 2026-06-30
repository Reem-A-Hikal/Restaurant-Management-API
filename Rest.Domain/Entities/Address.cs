using Rest.Domain.Entities.Enums;

namespace Rest.Domain.Entities
{
    /// <summary>
    /// Represents an address associated with a user.
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the first line of the address.
        /// </summary>
        public string AddressLine1 { get; private set; }

        /// <summary>
        /// Gets or sets the second line of the address (optional).
        /// </summary>
        public string? AddressLine2 { get; private set; }

        /// <summary>
        /// Gets or sets city name.
        /// </summary>
        public string City { get; private set; }

        public string? Governorate { get; private set; }

        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }

        /// <summary>
        /// Gets or sets the address type (e.g., Home, Work, Other).
        /// </summary>
        public AddressType AddressType { get; private set; }

        /// <summary>
        /// Is this address the default address for the user?
        /// </summary>
        public bool IsDefault { get; private set; }
        public string UserId { get; private set; }

        // Navigation properties
        public virtual User? User { get; set; }

        // Factory Method
        public static Address Create(
            string userId,
            string addressLine1,
            string? addressLine2,
            string city,
            string? governorate,
            double? latitude,
            double? longitude,
            AddressType addressType,
            bool isDefault = false)
        {
            if (string.IsNullOrWhiteSpace(addressLine1))
                throw new Exceptions.ValidationException("Address line 1 is required.");

            if (string.IsNullOrWhiteSpace(city))
                throw new Exceptions.ValidationException("City is required.");

            return new Address
            {
                UserId = userId,
                AddressLine1 = addressLine1,
                AddressLine2 = addressLine2,
                City = city,
                Governorate = governorate,
                Latitude = latitude,
                Longitude = longitude,
                AddressType = addressType,
                IsDefault = isDefault
            };
        }

        // Domain Method
        public void Update(
            string? addressLine1,
            string? addressLine2,
            string? city,
            string? governorate,
            double? latitude,
            double? longitude,
            AddressType? addressType)
        {
            if (!string.IsNullOrWhiteSpace(addressLine1))
                AddressLine1 = addressLine1;

            if (city != null)
            {
                if (string.IsNullOrWhiteSpace(city))
                    throw new Exceptions.ValidationException("City cannot be empty.");
                City = city;
            }
            AddressLine2 = addressLine2 ?? AddressLine2;
            Governorate = governorate ?? Governorate;
            Latitude = latitude ?? Latitude;
            Longitude = longitude ?? Longitude;

            if (addressType.HasValue)
                AddressType = addressType.Value;
        }

        public void SetAsDefault() => IsDefault = true;
        public void UnsetDefault() => IsDefault = false;
    }
}
