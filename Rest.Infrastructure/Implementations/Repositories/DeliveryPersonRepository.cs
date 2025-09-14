using Microsoft.EntityFrameworkCore;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class DeliveryPersonRepository :IDeliveryPersonRepository
    {
        private readonly RestDbContext _context;
        public DeliveryPersonRepository(RestDbContext context)
        {
            _context = context;
        }

        public async Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(string userId)
        {
            return await _context.DeliveryPeople.FirstOrDefaultAsync(d => d.Id == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
