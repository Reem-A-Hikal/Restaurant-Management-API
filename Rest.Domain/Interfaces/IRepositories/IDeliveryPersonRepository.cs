using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.IRepositories
{
    public interface IDeliveryPersonRepository
    {
        Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(string userId);
        Task SaveChangesAsync();
    }
}
