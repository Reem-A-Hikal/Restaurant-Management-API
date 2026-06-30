using Rest.Application.Dtos.AddressDtos;

namespace Rest.Application.Interfaces.IServices
{
    public interface IAddressService
    {
        Task<AddressCreateDto> CreateAddress(string userId, AddressCreateDto addressDto);
        Task UpdateAddress(string userId, int addressId, AddressUpdateDto addressDto);
        Task DeleteAddressAsync(string userId, int addressId);
        Task<AddressDto?> GetUserAddress(string userId, int addressId);
        Task<IEnumerable<AddressDto>> GetUserAddresses(string userId);
        Task SetDefaultAddress(string userId, int addressId);
        Task<AddressDto?> GetUserDefaultAddress(string userId);
    }
}
