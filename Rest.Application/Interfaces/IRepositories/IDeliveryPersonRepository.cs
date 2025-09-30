using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IDeliveryPersonRepository
    {
        Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(string userId);
        Task SaveChangesAsync();
    }
}
