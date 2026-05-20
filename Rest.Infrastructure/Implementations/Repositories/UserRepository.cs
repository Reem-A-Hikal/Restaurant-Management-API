using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;

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

        public IQueryable<User> GetFilteredUsers(string? searchTerm, string? selectedRole = "All")
        {
            var query = GetAllQueryable();

            if (!string.IsNullOrEmpty(selectedRole) && selectedRole != "All")
            {
                query = from u in query
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where r.Name == selectedRole
                        select u;
            }

            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.FullName.Contains(searchTerm));
            }
            return query.OrderByDescending(u => u.JoinDate);
        }
        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id)
                ?? throw new KeyNotFoundException("User not found");
        }
        
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email)
                ?? throw new KeyNotFoundException("User not found");
            
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
