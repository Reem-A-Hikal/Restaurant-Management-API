using Microsoft.EntityFrameworkCore;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;
using Rest.Infrastructure.Data;
using System.Threading.Tasks;

namespace Rest.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<User> _repository;

        public UserRepository(RestDbContext context, IRepository<User> repository)
        {
            _context = context;
            _repository = repository;
        }

        public IQueryable<User> GetAllQueryable() =>
            _context.Users.AsQueryable();

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
            return user ?? throw new KeyNotFoundException("User not found");
        }
        public async Task<Chef?> GetChefByIdAsync(string userId)
        {
            return await _context.Chefs.FirstOrDefaultAsync(c => c.Id == userId);
        }

        public async Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(string userId)
        {
            return await _context.DeliveryPeople.FirstOrDefaultAsync(d => d.Id == userId);
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }
        // Methods from IRepository
        public async Task AddAsync(User entity) => await _repository.AddAsync(entity);

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public async Task<IEnumerable<User>> GetAllAsync() => await _repository.GetAllAsync();

        public Task<User> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public void Update(User entity) => _repository.Update(entity);
        public Task SaveChangesAsync() => _repository.SaveChangesAsync();
    }
}
