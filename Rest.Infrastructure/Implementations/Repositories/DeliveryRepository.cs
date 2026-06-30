using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<Delivery> _repository;

        public DeliveryRepository(RestDbContext context, IRepository<Delivery> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task AddAsync(Delivery entity) => await _repository.AddAsync(entity);

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public void Update(Delivery entity) => _repository.Update(entity);

        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();

        public async Task<Delivery?> GetByIdAsync(int id)
        {
            return await _context.Deliveries
                .Include(d => d.DeliveryPerson)
                .Include(d => d.Order)
                .FirstOrDefaultAsync(d => d.DeliveryId == id);
        }

        public async Task<IEnumerable<Delivery>> GetAllAsync()
        {
            return await _context.Deliveries
                .Include(d => d.DeliveryPerson)
                .Include(d => d.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Delivery>> GetDeliveryHistoryByOrderIdAsync(int orderId)
        {
            return await _context.Deliveries
                .Include(d => d.DeliveryPerson)
                .Where(d => d.OrderId == orderId)
                .OrderByDescending(d => d.StatusChangeTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Delivery>> GetAllActiveDeliveriesAsync()
        {
            return await _context.Deliveries
                .Where(d => d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickedUp)
                .Include(d => d.DeliveryPerson)
                .Include(d => d.Order)
                .OrderBy(d => d.StatusChangeTime)
                .ToListAsync();
        }

        public async Task<Delivery?> GetActiveDeliveryByOrderIdAsync(int orderId)
        {
            return await _context.Deliveries
                .Include(d => d.DeliveryPerson)
                .Where(d => d.OrderId == orderId
                    && (d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickedUp))
                .OrderByDescending(d => d.StatusChangeTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Delivery>> GetActiveDeliveriesByPersonIdAsync(string deliveryPersonId)
        {
            return await _context.Deliveries
                .Include(d => d.Order)
                    .ThenInclude( o => o!.DeliveryAddress)
                .Where(d => d.DeliveryPersonId == deliveryPersonId
                    && (d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickedUp))
                .OrderBy(d => d.StatusChangeTime)
                .ToListAsync();
        }

        public async Task<string?> GetAvailableDeliveryPersonAsync()
        {
            var busyPersonIds = await _context.Deliveries
                .Where(d => d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickedUp)
                .Select(d => d.DeliveryPersonId)
                .Distinct()
                .ToListAsync();

            return await _context.DeliveryPersons
                .Where(dp => dp.IsAvailable == true && !busyPersonIds.Contains(dp.Id))
                .Select(dp => dp.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasActiveDeliveryAsync(string deliveryPersonId)
        {
            return await _context.Deliveries
                .AnyAsync(d => d.DeliveryPersonId == deliveryPersonId
                    && (d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickedUp));
        }
    }
}
