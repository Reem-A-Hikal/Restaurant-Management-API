using Rest.Application.Dtos.UserDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices.StrategyFactory
{
    public interface IRoleStrategy
    {
        string RoleName { get; }

        /// <summary>
        /// Creates the correct User entity based on the role
        /// Chef → creates Chef entity
        /// DeliveryPerson → creates DeliveryPerson entity
        /// Admin/Customer → creates base User entity
        /// </summary>
        User CreateUserEntity(CreateUserDto dto);

        /// <summary>
        /// Enriches the UserDto with role-specific data
        /// Called when returning user data to the client
        /// </summary>
        Task EnrichDtoAsync(UserDto dto);

        /// <summary>
        /// Updates role-specific data
        /// Chef → updates Specialization
        /// DeliveryPerson → updates VehicleNumber, IsAvailable
        /// Admin/Customer → does nothing
        /// </summary>
        Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto);
    }
}
