using Microsoft.EntityFrameworkCore;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Utilities;
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

        public async Task BulkEnrichChefsAsync(List<UserDto> userDtos)
        {
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
