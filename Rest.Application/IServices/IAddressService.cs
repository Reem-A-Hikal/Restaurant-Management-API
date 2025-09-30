
using Rest.Application.Dtos.AddressDtos;

namespace Rest.Application.IServices
{
    public interface IAddressService
    {
        Task<AddressCreateDto> CreateAddress(string userId, AddressCreateDto addressDto);
        Task<bool> UpdateAddress(string userId, int addressId, AddressUpdateDto addressDto);
        Task<bool> DeleteAddressAsync(string userId, int addressId);
        Task<AddressDto?> GetUserAddress(string userId, int addressId);
        Task<IEnumerable<AddressDto>> GetUserAddresses(string userId);
        Task<bool> SetDefaultAddress(string userId, int addressId);
        Task<AddressDto?> GetUserDefaultAddress(string userId);
    }
}
