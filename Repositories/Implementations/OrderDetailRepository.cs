using Microsoft.EntityFrameworkCore;
using Rest.API.Models;
using Rest.API.Repositories.Interfaces;
using System.Threading.Tasks;

namespace Rest.API.Repositories.Implementations
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly RestDbContext _context;
        IRepository<OrderDetail> _repository;
        public OrderDetailRepository(RestDbContext restDbContext, IRepository<OrderDetail> repository)
        {
            _context = restDbContext;
            _repository = repository;
        }

        public async Task AddAsync(OrderDetail entity) => await _repository.AddAsync(entity);

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .ToListAsync();
        }

        public async Task<OrderDetail> GetByIdAsync(int id) =>   await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .Where(od => od.OrderId == orderId).ToListAsync();
        }

        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();

        public  void Update(OrderDetail entity) => _repository.Update(entity);
    }
}

