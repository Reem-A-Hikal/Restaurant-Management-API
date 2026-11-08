using AutoMapper;
using Microsoft.Extensions.Options;
using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Application.Dtos.OrderDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Options;
using Rest.Domain.Constants;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class OrderService :IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly OrderPricingOptions _pricing;
        private readonly IKitchenSchedule _kitchen;
        private readonly IDeliveryZone _deliveryZone;
        private readonly DeliveryEstimateOptions _estimate;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IOptions<OrderPricingOptions> pricing,
            IKitchenSchedule kitchen,
            IDeliveryZone deliveryZone,
           IOptions<DeliveryEstimateOptions> estimate)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pricing = pricing.Value;
            _kitchen = kitchen;
            _deliveryZone = deliveryZone;
            _estimate = estimate.Value;
        }
        #region Private Methods
        private async Task<Address> ValidateCustomerAddressAsync(string customerId, int deliveryAddressId)
        {
            var customerAddresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(customerId);

            if (!customerAddresses.Any())
                throw new BusinessException(
                    "You don't have any saved addresses. Please add a delivery address before placing an order.",
                        ErrorCodes.NoSavedAddress);

            var deliveryAddress = customerAddresses.FirstOrDefault(a => a.AddressId == deliveryAddressId)
                ?? throw new ForbiddenException(
                    "The selected address does not belong to this customer.",
                    ErrorCodes.AddressNotOwned);

            return deliveryAddress;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
        #endregion

        public async Task<OrderDto> CreateOrderAsync(string userId,CreateOrderDto orderDto)
        {
            if (!_kitchen.IsAcceptingOrders(DateTime.UtcNow))
                throw new BusinessException(
                    $"We take orders daily between {_kitchen.OpensAt:hh\\:mm} and {_kitchen.ClosesAt:hh\\:mm} (Cairo time).",
                    ErrorCodes.KitchenClosed,
                    new Dictionary<string, object?>
                    {
                        ["opensAt"] = _kitchen.OpensAt.ToString(@"hh\:mm"),
                        ["closesAt"] = _kitchen.ClosesAt.ToString(@"hh\:mm"),
                    });

            User customer = await _unitOfWork.UserRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            Address deliveryAddress = await ValidateCustomerAddressAsync(customer.Id, orderDto.DeliveryAddressId);

            if (!_deliveryZone.IsDeliverable(deliveryAddress.City, deliveryAddress.Governorate))
                throw new BusinessException(
                    "We don't deliver to this address yet. Please choose an address inside our delivery areas.",
                    ErrorCodes.AddressOutsideDeliveryArea);

            if (orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
                throw new ValidationException("Order must contain at least one item.", ErrorCodes.OrderEmpty);

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = customer.Id,
                User = customer,
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress,
                CustomerNotes = orderDto.CustomerNotes,
                EstimatedDeliveryTime = _estimate.MaxMinutes
            };

            var categoryStatuses = new Dictionary<int, CategoryStatus>();

            foreach (var item in orderDto.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId)
                ?? throw new BusinessException(
                    "An item in your cart is no longer available. Please remove it and try again.",
                    ErrorCodes.ProductUnavailable,
                    new Dictionary<string, object?> { ["productId"] = item.ProductId });

                if (!categoryStatuses.TryGetValue(product.CategoryId, out var categoryStatus))
                {
                    var category = await _unitOfWork.CategoryRepository.GetByIdAsync(product.CategoryId);
                    categoryStatus = category?.Status ?? CategoryStatus.Archived;
                    categoryStatuses[product.CategoryId] = categoryStatus;
                }

                if (!product.IsAvailable || categoryStatus != CategoryStatus.Active)
                    throw new BusinessException(
                        $"'{product.Name}' is no longer available. Please remove it from your cart and try again.",
                        ErrorCodes.ProductUnavailable,
                        new Dictionary<string, object?>
                        {
                            ["productId"] = product.ProductId,
                            ["productName"] = product.Name,
                        });

                order.AddItem(product, item.Quantity);
            }
            order.ApplyCharges(_pricing.DeliveryFee); // tax/discount = 0 (no tax model yet)
            await _unitOfWork.OrderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> ConfirmOrderAsync(int orderId, string confirmedBy, ConfirmOrderDto confirmDto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            order.Confirm(confirmedBy, confirmDto.RequiredTime, confirmDto.StaffNotes);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CancelOrderAsync(int orderId, string cancellationReason, string actorUserId, bool isCustomer)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            if (isCustomer && order.UserId != actorUserId)
                throw new ForbiddenException();

            order.Cancel(cancellationReason,isCustomer);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }


        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdFullAsync(id)
                ?? throw new NotFoundException("Order", id);

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId)
        {
            var orders = await _unitOfWork.OrderRepository.GetOrdersByCustomerAsync(customerId)
                ?? [];

            return _mapper.Map<OrderDto[]>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status, string role)
        {
            if (!Enum.IsDefined(status))
                throw new ValidationException("Invalid order status.");

            var allowed = GetAllowedStatusesForRole(role).ToList();
            if (!allowed.Contains(status))
                throw new ForbiddenException(allowed.Count > 0
                    ? $"Your role can only view orders with status: {string.Join(", ", allowed)}."
                    : $"Role '{role}' is not permitted to view orders.");

            var orders = await _unitOfWork.OrderRepository.GetOrdersByStatusAsync(status)
                ?? [];

            return _mapper.Map<OrderDto[]>(orders);
        }

        public IEnumerable<OrderStatus> GetAllowedStatusesForRole(string role)
        {
            if (role == AppRoles.Admin)
                return Enum.GetValues<OrderStatus>();

            return _roleVisibleStatuses.TryGetValue(role, out var statuses)
                ? statuses
                : [];
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersVisibleToRoleAsync(string role)
        {
            if (role == AppRoles.Admin)
                return await GetAllOrdersAsync();

            if(_roleVisibleStatuses.TryGetValue(role, out var allowedStatuses))
            {
                var orders = await _unitOfWork.OrderRepository.GetOrdersByStatusesAsync(allowedStatuses);
                return _mapper.Map<OrderDto[]>(orders);
            }

            throw new ForbiddenException($"Role '{role}' is not permitted to view orders.");
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetAllAsync() ?? [];

            return _mapper.Map<OrderDto[]>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetKitchenQueueAsync()
        {
            var orders = await _unitOfWork.OrderRepository.GetKitchenQueueAsync() ?? [];
            return _mapper.Map<OrderDto[]>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var endExclusive = endDate.TimeOfDay == TimeSpan.Zero ? endDate.AddDays(1) : endDate;

            var orders = await _unitOfWork.OrderRepository.GetOrdersByDateRangeAsync(startDate, endExclusive) ?? [];
            return _mapper.Map<OrderDto[]>(orders);
        }

        public async Task<decimal> GetDailyRevenueAsync(DateTime date)
        {
            return await _unitOfWork.OrderRepository.GetDailyRevenueAsync(date);
        }
        
        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            return await _unitOfWork.OrderRepository.GetOrderCountByStatusAsync(status);
        }

        public async Task<OrderDto> GetOrderForUserAsync(int id, string userId, string role)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Order", id);

            var allowed = role switch
            {
                AppRoles.Admin => true,
                AppRoles.Customer => order.UserId == userId,
                AppRoles.Chef => _roleVisibleStatuses[AppRoles.Chef].Contains(order.Status),
                AppRoles.DeliveryPerson => order.Deliveries.Any(s => s.DeliveryPersonId == userId),
                _ => false
            };

            if(!allowed)
                throw new ForbiddenException();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> MarkAsPreparingAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            order.TransitionTo(OrderStatus.Preparing);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> MarkAsReadyAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            order.TransitionTo(OrderStatus.Ready);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        private static readonly Dictionary<string, OrderStatus[]> _roleVisibleStatuses = new()
        {
            [AppRoles.Chef] = new[] 
            { 
                OrderStatus.New,
                OrderStatus.Confirmed,
                OrderStatus.Preparing,
                OrderStatus.Ready
            },
        };
    }
}
