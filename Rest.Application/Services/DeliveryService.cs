using AutoMapper;
using Rest.Application.Dtos.DeliveryDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeliveryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DeliveryDto> AssignDeliveryAsync(int orderId, AssignDeliveryDto dto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            if (!order.CanTransitionTo(OrderStatus.OutForDelivery))
                throw new BusinessException(
                    $"Cannot assign delivery. Order status is '{order.Status}'.");

            var activeDelivery = await _unitOfWork.DeliveryRepository.GetActiveDeliveryByOrderIdAsync(orderId);
            if (activeDelivery != null)
                throw new BusinessException("Order already has an active delivery in progress.");

            DeliveryPerson chosen;
            if (!string.IsNullOrWhiteSpace(dto.DeliveryPersonId))
            {
                chosen = await _unitOfWork.DeliveryPersonRepository.GetDeliveryPersonByIdAsync(dto.DeliveryPersonId)
                    ?? throw new NotFoundException("DeliveryPerson", dto.DeliveryPersonId);

                if (!chosen.IsActive || chosen.IsAvailable != true)
                    throw new BusinessException("The selected delivery person is not available.");

                if (await _unitOfWork.DeliveryRepository.HasActiveDeliveryAsync(chosen.Id))
                    throw new BusinessException("The selected delivery person already has an active delivery.");
            }
            else
            {
                var availableDeliveryPersons = (await _unitOfWork.DeliveryRepository.GetAvailableDeliveryPersonsAsync()).ToList();
                chosen = availableDeliveryPersons.FirstOrDefault()
                    ?? throw new BusinessException("No delivery persons are available at the moment.");
            }
            var deliveryPersonId = chosen.Id;

            var delivery = new Delivery
            {
                OrderId = orderId,
                DeliveryPersonId = deliveryPersonId,
                StatusChangeTime = DateTime.UtcNow
            };

            await _unitOfWork.DeliveryRepository.AddAsync(delivery);

            order.TransitionTo(OrderStatus.OutForDelivery);
            
            _unitOfWork.OrderRepository.Update(order);
            
            chosen.MarkBusy();

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.DeliveryRepository.GetByIdAsync(delivery.DeliveryId);
            return _mapper.Map<DeliveryDto>(result);
        }

        public async Task<DeliveryDto> MarkAsPickedUpAsync(int deliveryId, string deliveryPersonId)
        {
            var delivery = await GetOwnedDeliveryAsync(deliveryId, deliveryPersonId);
            
            delivery.MarkAsPickedUp();

            _unitOfWork.DeliveryRepository.Update(delivery);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto> MarkAsDeliveredAsync(int deliveryId, string deliveryPersonId)
        {
            var delivery = await GetOwnedDeliveryAsync(deliveryId, deliveryPersonId);

            if (delivery.DeliveryPersonId != deliveryPersonId)
                throw new ForbiddenException("You can only update your own deliveries.");

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(delivery.OrderId)
                ?? throw new NotFoundException("Order", delivery.OrderId);

            var deliveryPerson = await _unitOfWork.DeliveryPersonRepository.GetDeliveryPersonByIdAsync(delivery.DeliveryPersonId)
                ?? throw new NotFoundException("DeliveryPerson", delivery.DeliveryPersonId);
            
            delivery.MarkAsDelivered();
            
            order.TransitionTo(OrderStatus.Delivered);

            if(!await _unitOfWork.PaymentRepository.HasCompletedPaymentAsync(order.OrderId))
            {
                var payment = Payment.CashCollected(order);
                await _unitOfWork.PaymentRepository.AddAsync(payment);
            }
            deliveryPerson.MarkAvailable();

            _unitOfWork.DeliveryRepository.Update(delivery);
            _unitOfWork.OrderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto> CancelDeliveryAsync(int deliveryId, string reason, string actorUserId, bool isAdmin)
        {
            var delivery = await GetOwnedDeliveryAsync(deliveryId, actorUserId, isAdmin);
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(delivery.OrderId)
                ?? throw new NotFoundException("Order", delivery.OrderId);

            delivery.Cancel(reason);

            order.RevertToReadyAfterDeliveryCancel();

            var deliveryPerson = await _unitOfWork.DeliveryPersonRepository.GetDeliveryPersonByIdAsync(delivery.DeliveryPersonId)
                ?? throw new NotFoundException("DeliveryPerson", delivery.DeliveryPersonId);

            deliveryPerson.MarkAvailable();

            _unitOfWork.DeliveryRepository.Update(delivery);
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<IEnumerable<DeliveryDto>> GetMyDeliveriesAsync(string deliveryPersonId)
        {
            var deliveries = await _unitOfWork.DeliveryRepository.GetActiveDeliveriesByPersonIdAsync(deliveryPersonId);
            return _mapper.Map<DeliveryDto[]>(deliveries);
        }

        public async Task<DeliveryDto?> GetDeliveryByIdAsync(int deliveryId)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId);
            return delivery == null ? null : _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto?> GetActiveDeliveryForOrderAsync(int orderId)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetActiveDeliveryByOrderIdAsync(orderId);
            return delivery == null ? null : _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<IEnumerable<DeliveryDto>> GetAllActiveDeliveriesAsync()
        {
            var deliveries = await _unitOfWork.DeliveryRepository.GetAllActiveDeliveriesAsync() ?? [];
            return _mapper.Map<IEnumerable<DeliveryDto>>(deliveries);
        }

        public async Task<IEnumerable<DeliveryDto>> GetDeliveryHistoryAsync(int orderId)
        {
            var deliveries = await _unitOfWork.DeliveryRepository.GetDeliveryHistoryByOrderIdAsync(orderId);
            return _mapper.Map<IEnumerable<DeliveryDto>>(deliveries);
        }

        public async Task<bool> HasActiveDeliveryAsync(string deliveryPersonId)
        {
            return await _unitOfWork.DeliveryRepository.HasActiveDeliveryAsync(deliveryPersonId);
        }

        public async Task<DeliveryDto> UpdateLocationAsync(int deliveryId, string deliveryPersonId, UpdateLocationDto dto)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId)
                ?? throw new NotFoundException("Delivery", deliveryId);

            if (delivery.DeliveryPersonId != deliveryPersonId)
                throw new ForbiddenException("You can only update your own delivery location");

            delivery.UpdateLocation(dto.Latitude, dto.Longitude);

            _unitOfWork.DeliveryRepository.Update(delivery);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<IEnumerable<AvailableDeliveryPersonDto>> GetAvailableDeliveryPersonsAsync()
        {
            var deliveryPersons = await _unitOfWork.DeliveryRepository.GetAvailableDeliveryPersonsAsync();
            return _mapper.Map<IEnumerable<AvailableDeliveryPersonDto>>(deliveryPersons);
        }

        private async Task<Delivery> GetOwnedDeliveryAsync(int deliveryId, string actorUserId, bool isAdmin = false)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId)
                ?? throw new NotFoundException("Delivery", deliveryId);

            if (!isAdmin && delivery.DeliveryPersonId != actorUserId)
                throw new ForbiddenException("You can only update your own deliveries.");

            return delivery;
        }
    }
}

