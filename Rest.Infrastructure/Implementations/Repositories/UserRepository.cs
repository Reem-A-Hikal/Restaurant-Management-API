using Microsoft.EntityFrameworkCore;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
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
            _context.Users
            .Where(u => u.Status != UserStatus.Deleted)
            .AsQueryable();

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
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.Status != UserStatus.Deleted)
                ?? throw new KeyNotFoundException("User not found");
        }
        
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Status != UserStatus.Deleted)
                ?? throw new KeyNotFoundException("User not found");
            
        }
        public async Task<Dictionary<string, string>> GetUsersRolesDictAsync(IEnumerable<string> userIds)
        {
            var userRoles = await (
                from ur in _context.UserRoles
                join r in _context.Roles on ur.RoleId equals r.Id
                where userIds.Contains(ur.UserId)
                select new { ur.UserId, r.Name }
            ).ToListAsync();

            return userRoles.GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Name).FirstOrDefault() ?? string.Empty
                );
        }

        public async Task BulkEnrichUsersAsync(List<UserDto> userDtos)
        {
            var userIds = userDtos.Select(u => u.Id).ToList();

            var chefIds = userDtos.Where(u => u.Role == AppRoles.Chef)
                .Select(u => u.Id)
                .ToList();

            if (chefIds.Count != 0)
            {
                var chefs = await _context.Chefs
                    .Where(c => chefIds.Contains(c.Id))
                    .ToDictionaryAsync(c => c.Id, c => c.Specialization);

                foreach (var dto in userDtos.Where(u => u.Role == AppRoles.Chef))
                    dto.Specialization = chefs.TryGetValue(dto.Id, out var spec) ? spec : null;
            }

            var deliveryIds = userDtos.Where(u => u.Role == AppRoles.DeliveryPerson)
                .Select(dp => dp.Id)
                .ToList();

            if (deliveryIds.Count != 0)
            {
                var deliveryPersons = await _context.DeliveryPersons
                    .Where(dp => deliveryIds.Contains(dp.Id))
                    .ToDictionaryAsync(dp => dp.Id, dp => new
                    {
                        dp.VehicleNumber,
                        dp.IsAvailable
                    });

                foreach (var dto in userDtos.Where(u => u.Role == AppRoles.DeliveryPerson))
                {
                    if (deliveryPersons.TryGetValue(dto.Id, out var dp))
                    {
                        dto.VehicleNumber = dp.VehicleNumber;
                        dto.IsAvailable = dp.IsAvailable;
                    }
                }
            }
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
