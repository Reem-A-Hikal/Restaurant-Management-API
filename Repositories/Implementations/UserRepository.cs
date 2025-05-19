using Microsoft.EntityFrameworkCore;
using Rest.API.Models;
using Rest.API.Repositories.Interfaces;

namespace Rest.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly RestDbContext context;
        private readonly IRepository<User> repository;

        public UserRepository(RestDbContext context, IRepository<User> repository)
        {
            this.context = context;
            this.repository = repository;
        }

        public async Task<User> GetByIdAsync(string id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            return user;
        }
        // Methods from IRepository
        public async Task AddAsync(User entity) => await repository.AddAsync(entity);

        public async Task DeleteAsync(int id) => await repository.DeleteAsync(id);

        public async Task<IEnumerable<User>> GetAllAsync() => await repository.GetAllAsync();

        public Task<User> GetByIdAsync(int id) => repository.GetByIdAsync(id);

        public Task SaveChangesAsync() => repository.SaveChangesAsync();

        public void Update(User entity) => repository.Update(entity);
    }
}
