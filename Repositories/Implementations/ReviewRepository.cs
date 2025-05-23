using Microsoft.EntityFrameworkCore;
using Rest.API.Models;
using Rest.API.Repositories.Interfaces;
using System;

namespace Rest.API.Repositories.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly RestDbContext _context;

        public ReviewRepository(RestDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Order)
                .Include(r => r.Product)
                .ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Order)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.ReviewId == id);
        }
        public async Task AddAsync(Review entity)
        {
            await _context.Reviews.AddAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _context.Reviews
                .Where(r => r.ReviewId == id)
                .ExecuteDeleteAsync();
        }



        public async Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId)
        {
            return await _context.Reviews
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Include(r => r.Customer)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(Review entity)
        {
            _context.Reviews.Update(entity);
        }
    }
}
