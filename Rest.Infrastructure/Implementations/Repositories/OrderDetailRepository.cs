using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<OrderDetail> _repository;
        public OrderDetailRepository(RestDbContext restDbContext, IRepository<OrderDetail> repository)
        {
            _context = restDbContext;
            _repository = repository;
        }

        public async Task AddAsync(OrderDetail entity) => await _repository.AddAsync(entity);

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public void Update(OrderDetail entity) => _repository.Update(entity);
        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();

        /// <summary>
        /// Lightweight fetch by Id — no Includes. Intended for internal use
        /// (e.g. editing a line item already known to belong to a loaded Order).
        /// </summary>
        public async Task<OrderDetail> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);
        
        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<OrderDetail?> GetByOrderIdAndProductIdAsync(int orderId, int productId)
        {
            return await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductId == productId);
        }
    }
}

