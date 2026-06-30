using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;
using Rest.Infrastructure.Helpers;
using System.Threading.Tasks;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IRepository<Review> _repository;
        private readonly RestDbContext _context;

        public ReviewRepository(RestDbContext context, IRepository<Review> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }
        public async Task<Review?> GetByOrderIdAsync(int orderId)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        public async Task<IEnumerable<Review>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Reviews
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Product)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        public async Task<PaginatedList<Review>> GetPaginatedAsync(int productId, int pageIndex, int pageSize)
        {
            var query = GetByProductIdQueryable(productId);
            return await PaginationHelper.CreateAsync(query, pageIndex, pageSize);
        }

        public IQueryable<Review> GetByProductIdQueryable(int productId)
        {
            return _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Customer)
                .OrderByDescending(r => r.ReviewDate)
                .AsQueryable();
        }

        public async Task<IEnumerable<Review>> GetAllAsync() => await _repository.GetAllAsync();
        public async Task AddAsync(Review entity) => await _repository.AddAsync(entity);
        public void Update(Review entity) => _repository.Update(entity);
        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
        public Task SaveChangesAsync() => _repository.SaveChangesAsync();
    }
}
