using AutoMapper;
using Rest.Application.Dtos.UserDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices.StrategyFactory;
using Rest.Domain.Constants;
using Rest.Domain.Entities;

namespace Rest.Infrastructure.Implementations.StrategyFactory
{
    public class DeliveryPersonStrategy : IRoleStrategy
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DeliveryPersonStrategy(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public string RoleName => AppRoles.DeliveryPerson;

        public User CreateUserEntity(CreateUserDto dto) => _mapper.Map<DeliveryPerson>(dto);
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
                deliveryPerson.VehicleNumber = dto.VehicleNumber;

            if (dto.IsAvailable.HasValue)
                deliveryPerson.IsAvailable = dto.IsAvailable.Value;

            await _unitOfWork.SaveChangesAsync();
            
        }
    }
}
