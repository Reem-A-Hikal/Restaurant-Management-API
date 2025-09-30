using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.Services.StrategyFactory
{
    public class DeliveryPersonStrategy : IRoleStrategy
    {
        private readonly IDeliveryPersonRepository _deliveryPersonRepo;
        private readonly IMapper _mapper;
        public DeliveryPersonStrategy(IDeliveryPersonRepository deliveryPersonRepo, IMapper mapper)
        {
            _deliveryPersonRepo = deliveryPersonRepo;
            _mapper = mapper;
        }
        public string RoleName => AppRoles.DeliveryPerson;

        public User CreateUserEntity(CreateUserDto dto)
            => _mapper.Map<DeliveryPerson>(dto);

        public async Task EnrichDtoAsync(UserDto dto)
        {
            var deliveryPerson = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(dto.Id);
            dto.VehicleNumber = deliveryPerson?.VehicleNumber;
            dto.IsAvailable = deliveryPerson?.IsAvailable;
        }

        public async Task UpdateRoleDataAsync(string userId, UpdateProfileDto dto)
        {
            var deliveryPerson = await _deliveryPersonRepo.GetDeliveryPersonByIdAsync(userId);
            if (deliveryPerson != null)
            {
                if (!string.IsNullOrWhiteSpace(dto.VehicleNumber))
                    deliveryPerson.VehicleNumber = dto.VehicleNumber;

                if (dto.IsAvailable.HasValue)
                    deliveryPerson.IsAvailable = dto.IsAvailable.Value;

                await _deliveryPersonRepo.SaveChangesAsync();
            }
        }
    }
}
