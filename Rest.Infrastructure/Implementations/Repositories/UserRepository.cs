using Microsoft.EntityFrameworkCore;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Utilities;
using Rest.Domain.Constants;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;
using Rest.Infrastructure.Helpers;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly RestDbContext _context;

        public UserRepository(RestDbContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetFilteredUsers(string? searchTerm, string? selectedRole = "All")
        {
            var adminUserIds = from ur in _context.UserRoles
                               join r in _context.Roles on ur.RoleId equals r.Id
                               where r.Name == AppRoles.Admin
                               select ur.UserId;

            var query = _context.Users
                .Where(u => !adminUserIds.Contains(u.Id))
                .AsQueryable();

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

        public async Task<PaginatedList<User>> GetPaginatedAsync(
            int pageIndex, int pageSize,
            string? searchTerm, string? selectedRole)
        {
            var query = GetFilteredUsers(searchTerm, selectedRole);
            return await PaginationHelper.CreateAsync(query, pageIndex, pageSize);
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
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
        public async Task<IEnumerable<User>> GetAllAsync() 
        {
            var adminUserIds = await (from ur in _context.UserRoles
                                      join r in _context.Roles on ur.RoleId equals r.Id
                                      where r.Name == AppRoles.Admin
                                      select ur.UserId).ToListAsync();

            return await _context.Users
                .Where(u => !adminUserIds.Contains(u.Id))
                .ToListAsync();
        }
        public async Task AddAsync(User entity) => await _context.Users.AddAsync(entity);
        public void Update(User entity) => _context.Users.Update(entity);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
