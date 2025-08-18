using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;

namespace Rest.Infrastructure.Implementations.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IRepository<Delivery> _deliveryRepository;

        public DeliveryService(IRepository<Delivery> deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        public async Task<IEnumerable<Delivery>> GetAllDeliveriesAsync()
        {
            return await _deliveryRepository.GetAllAsync();
        }

        public async Task<Delivery> GetDeliveryByIdAsync(int id)
        {
            return await _deliveryRepository.GetByIdAsync(id);
        }

        public async Task AddDeliveryAsync(Delivery delivery)
        {
            await _deliveryRepository.AddAsync(delivery);
            await _deliveryRepository.SaveChangesAsync();
        }

        public async Task UpdateDeliveryAsync(Delivery delivery)
        {
            _deliveryRepository.Update(delivery);
            await _deliveryRepository.SaveChangesAsync();
        }

        public async Task DeleteDeliveryAsync(int id)
        {
            await _deliveryRepository.DeleteAsync(id);
            await _deliveryRepository.SaveChangesAsync();
        }
    }
}

