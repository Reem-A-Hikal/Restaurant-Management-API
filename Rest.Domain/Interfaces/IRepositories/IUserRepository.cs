using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.IRepositories
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User> GetByIdAsync(string id);
        Task<Chef?> GetChefByIdAsync(string userId);
        Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(string userId);
        Task<User> GetByEmailAsync(string email);
    }
}
