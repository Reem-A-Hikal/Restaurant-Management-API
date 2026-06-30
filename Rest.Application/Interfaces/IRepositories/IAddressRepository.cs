
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IAddressRepository :IRepository<Address>
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(string userId);
        Task<Address?> GetUserAddressAsync(string userId, int addressId);
        Task<Address?> GetUserDefaultAddressAsync(string userId);
        Task SetDefaultAddressAsync(string userId, int addressId);
        Task<bool> HasAddressAsync(int addressId, string userId);
    }
}
