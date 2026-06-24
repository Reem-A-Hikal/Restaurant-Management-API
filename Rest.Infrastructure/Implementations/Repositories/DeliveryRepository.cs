using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<Delivery> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Delivery>> GetAllAsync()
        {
            return await _context.Deliveries
                .Include(d => d.DeliveryPerson)
                .Include(d => d.Order)
                .ToListAsync();
        }
        public async Task<Delivery?> GetActiveDeliveryByOrderIdAsync(int orderId)
        {
            return await _context.Deliveries
                .Where(d => d.OrderId == orderId
                    && (d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickedUp))
                .OrderByDescending(d => d.StatusChangeTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Delivery>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Deliveries
                .Where(d => d.OrderId == orderId)
                .Include(d => d.DeliveryPerson)
                .OrderBy(d => d.StatusChangeTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Delivery>> GetActiveDeliveriesByPersonIdAsync(string deliveryPersonId)
        {
            return await _context.Deliveries
                .Where(d => d.DeliveryPersonId == deliveryPersonId
                    && d.Status == DeliveryStatus.Assigned || d.Status == DeliveryStatus.PickedUp)
                .Include(d => d.Order)
                .OrderBy(d => d.StatusChangeTime)
                .ToListAsync();
        }
    }
}
