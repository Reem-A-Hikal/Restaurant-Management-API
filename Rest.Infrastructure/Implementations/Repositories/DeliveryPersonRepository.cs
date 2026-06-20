using Microsoft.EntityFrameworkCore;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Constants;
using Rest.Domain.Entities;
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

        public async Task BulkEnrichDeliveryPersonsAsync(List<UserDto> userDtos)
        {
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

        public async Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(string userId)
        {
            return await _context.DeliveryPersons.FirstOrDefaultAsync(d => d.Id == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
