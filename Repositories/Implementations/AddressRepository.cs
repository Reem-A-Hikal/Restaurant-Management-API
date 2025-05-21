using Microsoft.EntityFrameworkCore;
using Rest.API.Models;
using Rest.API.Repositories.Interfaces;

namespace Rest.API.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<Address> _repository;

        public AddressRepository(RestDbContext context, IRepository<Address> repository)
        {
            _context = context;
            _repository = repository;
        }
        public async Task<Address?> GetUserAddressAsync(string userId, int addressId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);
        }

        public async Task<IEnumerable<Address>> GetUserAddressesAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenBy(a => a.AddressType)
                .ToListAsync();
        }

        public async Task<Address?> GetUserDefaultAddressAsync(string userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && a.IsDefault);
        }

        public async Task<bool> SetDefaultAddressAsync(string userId, int addressId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Addresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ExecuteUpdateAsync(a => a.SetProperty(x => x.IsDefault, false));

                await _context.Addresses
                    .Where(a => a.UserId == userId && a.AddressId == addressId)
                    .ExecuteUpdateAsync(a => a.SetProperty(x => x.IsDefault, true));

                await transaction.CommitAsync();
                return true;
            }
            catch(Exception ex)
            {

                return false;

            }
        }
        public async Task<bool> HasAddressAsync(int addressId, string userId)
        {
            return await _context.Addresses
                .AnyAsync(a => a.AddressId == addressId && a.UserId == userId);
        }
        public async Task AddAsync(Address entity) => await _repository.AddAsync(entity);

        public async Task DeleteAsync(int addressId) => await _repository.DeleteAsync(addressId);

        public Task<IEnumerable<Address>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Address> GetByIdAsync(int id) => _repository.GetByIdAsync(id);


        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();


        public void Update(Address entity) => _repository.Update(entity);
    }
}