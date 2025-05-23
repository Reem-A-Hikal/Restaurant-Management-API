using Rest.API.Models;

namespace Rest.API.Services.Interfaces
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
