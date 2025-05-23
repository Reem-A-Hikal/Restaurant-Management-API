﻿using Rest.API.Models;

namespace Rest.API.Repositories.Interfaces
{
    public interface IAddressRepository :IRepository<Address>
    {
        Task<IEnumerable<Address>> GetUserAddressesAsync(string userId);
        Task<Address?> GetUserAddressAsync(string userId, int addressId);
        Task<Address?> GetUserDefaultAddressAsync(string userId);
        Task<bool> SetDefaultAddressAsync(string userId, int addressId);
        Task<bool> HasAddressAsync(int addressId, string userId);
    }
}
