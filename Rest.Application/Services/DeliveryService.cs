using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;

namespace Rest.Application.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IRepository<Delivery> _deliveryRepository;

        public DeliveryService(IRepository<Delivery> deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        //public async Task<IEnumerable<DeliveryDto>> GetAllDeliveriesAsync()
        //{
        //    var deliveries 
        //    return await _deliveryRepository.GetAllAsync();
        //}

        //public async Task<DeliveryDto> GetDeliveryByIdAsync(int id)
        //{
        //    return await _deliveryRepository.GetByIdAsync(id);
        //}

        //public async Task AddDeliveryAsync(DeliveryDto delivery)
        //{
        //    await _deliveryRepository.AddAsync(delivery);
        //    await _deliveryRepository.SaveChangesAsync();
        //}

        //public async Task UpdateDeliveryAsync(DeliveryDto delivery)
        //{
        //    _deliveryRepository.Update(delivery);
        //    await _deliveryRepository.SaveChangesAsync();
        //}

        //public async Task DeleteDeliveryAsync(int id)
        //{
        //    await _deliveryRepository.DeleteAsync(id);
        //    await _deliveryRepository.SaveChangesAsync();
        //}
    }
}

