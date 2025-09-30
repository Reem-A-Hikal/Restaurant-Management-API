using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class ChefRepository : IChefRepository
    {
        private readonly RestDbContext _context;
        public ChefRepository(RestDbContext context)
        {
            _context = context;
        }
        public async Task<Chef?> GetChefByIdAsync(string userId)
        {
            return await _context.Chefs.FirstOrDefaultAsync(c => c.Id == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
