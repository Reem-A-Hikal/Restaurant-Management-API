using Rest.Application.Dtos.UserDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices.StrategyFactory
{
    public interface IRoleStrategyResolver
    {
        IRoleStrategy Resolve(string role);

        /// <summary>
        /// Creates the correct User entity based on the role
        /// Chef → creates Chef entity
        /// DeliveryPerson → creates DeliveryPerson entity
        /// Admin/Customer → creates base User entity
        /// </summary>
        Task<User> CreateUserAsync(CreateUserDto dto);
    }
}
