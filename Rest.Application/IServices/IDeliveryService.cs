using Rest.Domain.Entities;

namespace Rest.Application.IServices
{
    public interface IDeliveryService
    {

        Task<IEnumerable<Delivery>> GetAllDeliveriesAsync();
        Task<Delivery> GetDeliveryByIdAsync(int id);
        Task AddDeliveryAsync(Delivery delivery);
        Task UpdateDeliveryAsync(Delivery delivery);
        Task DeleteDeliveryAsync(int id);
    }
}
