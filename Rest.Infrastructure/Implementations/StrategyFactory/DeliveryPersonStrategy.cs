using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Domain.Constants;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;

namespace Rest.Infrastructure.Implementations.StrategyFactory
{
    public class DeliveryPersonStrategy : IRoleStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeliveryPersonStrategy(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public string RoleName => AppRoles.DeliveryPerson;

        public User CreateUserEntity(CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.VehicleNumber))
                throw new ValidationException("Vehicle number is required for DeliveryPerson role.");

            return DeliveryPerson.Create(
                dto.Email,
                dto.UserName,
                dto.FullName,
                dto.PhoneNumber,
                dto.ProfileImageUrl,
                dto.VehicleNumber);
        }
        public async Task EnrichDtoAsync(UserDto dto)
        {
            var deliveryPerson = await _unitOfWork.DeliveryPersonRepository.GetDeliveryPersonByIdAsync(dto.Id);
            dto.VehicleNumber = deliveryPerson?.VehicleNumber;
            dto.IsAvailable = deliveryPerson?.IsAvailable;
        }

        public async Task UpdateRoleDataAsync(string userId, AdminUpdateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.VehicleNumber) && dto.IsAvailable == null) return;
            
            var deliveryPerson = await _unitOfWork.DeliveryPersonRepository.GetDeliveryPersonByIdAsync(userId);
            if (deliveryPerson == null) return;
            
            if (!string.IsNullOrWhiteSpace(dto.VehicleNumber))
                deliveryPerson.SetVehicleNumber(dto.VehicleNumber);

            if (dto.IsAvailable.HasValue)
                deliveryPerson.SetAvailability(dto.IsAvailable.Value);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
