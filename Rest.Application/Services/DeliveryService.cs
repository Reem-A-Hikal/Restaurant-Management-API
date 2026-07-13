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

            string deliveryPersonId;
            if (!string.IsNullOrWhiteSpace(dto.DeliveryPersonId))
            {
                var isBusy = await _unitOfWork.DeliveryRepository.HasActiveDeliveryAsync(dto.DeliveryPersonId);
                if (isBusy)
                    throw new BusinessException("The selected delivery person already has an active delivery.");

                deliveryPersonId = dto.DeliveryPersonId;
            }
            else
            {
                var availableDeliveryPersons = await _unitOfWork.DeliveryRepository.GetAvailableDeliveryPersonsAsync();

                if (availableDeliveryPersons is null || !availableDeliveryPersons.Any())
                    throw new BusinessException("No delivery persons are available at the moment.");


                deliveryPersonId = availableDeliveryPersons.First().Id;
            }

            var delivery = new Delivery
            {
                OrderId = orderId,
                DeliveryPersonId = deliveryPersonId,
                StatusChangeTime = DateTime.UtcNow
            };

            await _unitOfWork.DeliveryRepository.AddAsync(delivery);

            order.TransitionTo(OrderStatus.OutForDelivery);
            
            _unitOfWork.OrderRepository.Update(order);
            
            var deliveryPerson = await _unitOfWork.DeliveryPersonRepository.GetDeliveryPersonByIdAsync(deliveryPersonId)
                ?? throw new NotFoundException("DeliveryPerson", deliveryPersonId);
            
            deliveryPerson.MarkBusy();

            await _unitOfWork.SaveChangesAsync();

            var result = await _unitOfWork.DeliveryRepository.GetByIdAsync(delivery.DeliveryId);
            return _mapper.Map<DeliveryDto>(result);
        }

        public async Task<DeliveryDto> MarkAsPickedUpAsync(int deliveryId)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId)
                ?? throw new NotFoundException("Delivery", deliveryId);
            
            delivery.MarkAsPickedUp();

            _unitOfWork.DeliveryRepository.Update(delivery);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto> MarkAsDeliveredAsync(int deliveryId)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId)
                ?? throw new NotFoundException("Delivery", deliveryId);

            var order = await _unitOfWork.OrderRepository.GetByIdAsync(delivery.OrderId)
                ?? throw new NotFoundException("Order", delivery.OrderId);

            delivery.MarkAsDelivered();
            
            order.TransitionTo(OrderStatus.Delivered);

            if(order.PaymentStatus != PaymentStatus.Completed)
            {
                var payment = new Payment
                {
                    OrderId = order.OrderId,
                    Order = order,
                    Amount = order.TotalAmount,
                    Method = PaymentMethod.Cash
                };
                payment.Complete();
                await _unitOfWork.PaymentRepository.AddAsync(payment);
            }

            _unitOfWork.DeliveryRepository.Update(delivery);
            _unitOfWork.OrderRepository.Update(order);

            var deliveryPerson = await _unitOfWork.DeliveryPersonRepository.GetDeliveryPersonByIdAsync(delivery.DeliveryPersonId)
                ?? throw new NotFoundException("DeliveryPerson", delivery.DeliveryPersonId);
            
            deliveryPerson.MarkAvailable();

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DeliveryDto>(delivery);
        }

        public async Task<DeliveryDto> CancelDeliveryAsync(int deliveryId, string reason)
        {
            var delivery = await _unitOfWork.DeliveryRepository.GetByIdAsync(deliveryId)
                ?? throw new NotFoundException("Delivery", deliveryId);

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
    }
}

