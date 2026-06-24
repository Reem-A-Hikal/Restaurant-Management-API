using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<Payment> _repository;

        public PaymentRepository(RestDbContext context, IRepository<Payment> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task AddAsync(Payment entity) => await _repository.AddAsync(entity);

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public void Update(Payment entity) => _repository.Update(entity);

        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();

        public async Task<Payment> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .Include(p => p.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasCompletedPaymentAsync(int orderId)
        {
            return await _context.Payments
                .AnyAsync(p => p.OrderId == orderId && p.Status == PaymentStatus.Completed);
        }

        public async Task<Payment?> GetCompletedPaymentAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId && p.Status == PaymentStatus.Completed)
                .OrderByDescending(p => p.PaidAt)
                .FirstOrDefaultAsync();
        }
    }
}