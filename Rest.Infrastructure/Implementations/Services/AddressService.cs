using AutoMapper;
using Rest.Application.Dtos.AddressDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.Services
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
        /// <returns> A task representing the asynchronous operation, with the created address as the result.</returns>
        public async Task<AddressCreateDto> CreateAddress(string userId, AddressCreateDto addressDto)
        {
            try
            {
                var address = _mapper.Map<Address>(addressDto);
                address.UserId = userId;

                var userAddresses = await _addressRepository.GetUserAddressesAsync(userId);
                if (userAddresses == null || !userAddresses.Any())
                {
                    address.IsDefault = true;
                }
                else
                {
                    address.IsDefault = false;
                }

                await _addressRepository.AddAsync(address);
                await _addressRepository.SaveChangesAsync();

                if (addressDto.IsDefault == true)
                {
                    await SetDefaultAddress(userId, address.AddressId);
                }

                return addressDto;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the address.", ex);
            }
        }

        /// <summary>
        /// Updates an existing address for the user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <param name="addressId"> The ID of the address to update.</param>
        /// <param name="addressDto"> The address data transfer object containing the updated address details.</param>
        /// <returns> A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
        public async Task<bool> UpdateAddress(string userId, int addressId, AddressUpdateDto addressDto)
        {
            try
            {
                var address = await _addressRepository.GetUserAddressAsync(userId, addressId);
                if (address == null)
                {
                    return false;
                }
                _mapper.Map(addressDto, address);
                _addressRepository.Update(address);
                await _addressRepository.SaveChangesAsync();

                if (addressDto.IsDefault == true)
                {
                    return await SetDefaultAddress(userId, addressId);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the address.", ex);
            }
        }

        /// <summary>
        /// Deletes an address for the user.
        /// </summary>
        /// <param name="addressId"> The ID of the address to delete.</param>
        /// <param name="userId"> The ID of the user.</param>
        /// <returns> A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
        public async Task<bool> DeleteAddressAsync(string userId, int addressId)
        {
            try
            {
                var address = await _addressRepository.GetUserAddressAsync(userId, addressId);
                if (address == null)
                {
                    return false;
                }
                await _addressRepository.DeleteAsync(addressId);
                await _addressRepository.SaveChangesAsync();

                if (address.IsDefault)
                {
                    var addresses = await _addressRepository.GetUserAddressesAsync(userId);
                    var newDefaultAddress = addresses.FirstOrDefault(a => a.AddressId != addressId);
                    if (newDefaultAddress != null)
                    {
                        await SetDefaultAddress(userId, newDefaultAddress.AddressId);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the address.", ex);
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
            var addressDto =_mapper.Map<AddressDto>(address);

            if (address == null)
            {
                return null;
            }
            return addressDto;
        }

        /// <summary>
        /// Retrieves all addresses for a specific user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <returns> A task representing the asynchronous operation, with a collection of addresses as the result.</returns>
        public async Task<IEnumerable<AddressDto>> GetUserAddresses(string userId)
        {
            var addresses = await _addressRepository.GetUserAddressesAsync(userId);
            var addressDtos = _mapper.Map<IEnumerable<AddressDto>>(addresses);
            if (addresses == null)
            {
                return Enumerable.Empty<AddressDto>();
            }
            return addressDtos;
        }

        /// <summary>
        /// Retrieves the default address for a specific user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <returns> A task representing the asynchronous operation, with the default address as the result.</returns>
        public async Task<AddressDto?> GetUserDefaultAddress(string userId)
        {
            var address = await _addressRepository.GetUserDefaultAddressAsync(userId);
            var addressDto = _mapper.Map<AddressDto>(address);
            if (address == null)
            {
                return null;
            }
            return addressDto;
        }
        /// <summary>
        /// Sets the default address for a specific user.
        /// </summary>
        /// <param name="userId"> The ID of the user.</param>
        /// <param name="addressId"> The ID of the address to set as default.</param>
        /// <returns> A task representing the asynchronous operation, with a boolean indicating success or failure.</returns>
        public async Task<bool> SetDefaultAddress(string userId, int addressId)
        {
            return await _addressRepository.SetDefaultAddressAsync(userId, addressId);
        }

    }
}
