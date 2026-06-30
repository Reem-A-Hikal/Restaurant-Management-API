using AutoMapper;
using Rest.Application.Dtos.AddressDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    /// <summary>
    /// Service for managing addresses.
    /// </summary>
    public class AddressService :IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressService"/> class.
        /// </summary>
        public AddressService(IAddressRepository repository, IMapper mapper)
        {
            _addressRepository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new address for the user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <param name="addressDto"> The address data transfer object containing the address details.</param>
        /// <returns> A task representing the asynchronous operation.</returns>
        public async Task<AddressCreateDto> CreateAddress(string userId, AddressCreateDto addressDto)
        {
            var userAddresses = await _addressRepository.GetUserAddressesAsync(userId);
            var isFirstAddress = !userAddresses.Any();

            var address = Address.Create(
                userId,
                addressDto.AddressLine1,
                addressDto.AddressLine2,
                addressDto.City,
                addressDto.Governorate,
                addressDto.Latitude,
                addressDto.Longitude,
                addressDto.AddressType ?? AddressType.Home,
                isDefault: isFirstAddress);

            await _addressRepository.AddAsync(address);
            await _addressRepository.SaveChangesAsync();

            if (addressDto.IsDefault && !isFirstAddress)
                await _addressRepository.SetDefaultAddressAsync(userId, address.AddressId);

            return addressDto;
        }

        /// <summary>
        /// Updates an existing address for the user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <param name="addressId"> The ID of the address to update.</param>
        /// <param name="addressDto"> The address data transfer object containing the updated address details.</param>
        /// <returns> A task representing the asynchronous operation.</returns>
        public async Task UpdateAddress(string userId, int addressId, AddressUpdateDto addressDto)
        {
            
            var address = await _addressRepository.GetUserAddressAsync(userId, addressId)
                ?? throw new NotFoundException("Address", addressId);

            address.Update(
            addressDto.AddressLine1,
            addressDto.AddressLine2,
            addressDto.City,
            addressDto.Governorate,
            addressDto.Latitude,
            addressDto.Longitude,
            addressDto.AddressType);

            _addressRepository.Update(address);
            await _addressRepository.SaveChangesAsync();

            if (addressDto.IsDefault == true)
                await _addressRepository.SetDefaultAddressAsync(userId, addressId);
        }

        /// <summary>
        /// Deletes an address for the user.
        /// </summary>
        /// <param name="addressId"> The ID of the address to delete.</param>
        /// <param name="userId"> The ID of the user.</param>
        /// <returns> A task representing the asynchronous operation.</returns>
        public async Task DeleteAddressAsync(string userId, int addressId)
        {
            var address = await _addressRepository.GetUserAddressAsync(userId, addressId)
                ?? throw new NotFoundException("Address", addressId);

            var wasDefault = address.IsDefault;

            await _addressRepository.DeleteAsync(addressId);
            await _addressRepository.SaveChangesAsync();

            if (wasDefault)
            {
                var addresses = await _addressRepository.GetUserAddressesAsync(userId);
                var newDefaultAddress = addresses.FirstOrDefault();
                if (newDefaultAddress != null)
                    await SetDefaultAddress(userId, newDefaultAddress.AddressId);
            }
        }
        /// <summary>
        /// Retrieves a specific address for a user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <param name="addressId"> The ID of the address to retrieve.</param>
        /// <returns> A task representing the asynchronous operation, with the address as the result.</returns>
        public async Task<AddressDto?> GetUserAddress(string userId, int addressId)
        {
            var address = await _addressRepository.GetUserAddressAsync(userId, addressId);
            return address == null ? null : _mapper.Map<AddressDto>(address);
        }

        /// <summary>
        /// Retrieves all addresses for a specific user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <returns> A task representing the asynchronous operation, with a collection of addresses as the result.</returns>
        public async Task<IEnumerable<AddressDto>> GetUserAddresses(string userId)
        {
            var addresses = await _addressRepository.GetUserAddressesAsync(userId);
            return _mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        /// <summary>
        /// Retrieves the default address for a specific user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <returns> A task representing the asynchronous operation, with the default address as the result.</returns>
        public async Task<AddressDto?> GetUserDefaultAddress(string userId)
        {
            var address = await _addressRepository.GetUserDefaultAddressAsync(userId);
            return address == null ? null : _mapper.Map<AddressDto>(address);
        }
        /// <summary>
        /// Sets the default address for a specific user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <param name="addressId"> The ID of the address to set as default.</param>
        public async Task SetDefaultAddress(string userId, int addressId)
        {
            await _addressRepository.SetDefaultAddressAsync(userId, addressId);
        }
    }
}
