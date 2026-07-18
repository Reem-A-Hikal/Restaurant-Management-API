using AutoMapper;
using Rest.Application.Dtos.OrderDetailsDtos;
using Rest.Application.Dtos.OrderDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IServices;
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
        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region Private Methods
        private async Task<Address> ValidateCustomerAddressAsync(string customerId, int deliveryAddressId)
        {
            var customerAddresses = await _unitOfWork.AddressRepository.GetUserAddressesAsync(customerId);

            if (!customerAddresses.Any())
                throw new BusinessException("You don't have any saved addresses. Please add a delivery address before placing an order.");

            var deliveryAddress = customerAddresses.FirstOrDefault(a => a.AddressId == deliveryAddressId)
                ?? throw new ForbiddenException("The selected address does not belong to this customer.");

            return deliveryAddress;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }
        #endregion

        public async Task<OrderDto> CreateOrderAsync(string userId,CreateOrderDto orderDto)
        {
            User customer = await _unitOfWork.UserRepository.GetByIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            Address deliveryAddress = await ValidateCustomerAddressAsync(customer.Id, orderDto.DeliveryAddressId);

            if (orderDto.OrderDetails == null || !orderDto.OrderDetails.Any())
                throw new ValidationException("Order must contain at least one item.");

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = customer.Id,
                User = customer,
                DeliveryAddressId = deliveryAddress.AddressId,
                DeliveryAddress = deliveryAddress,
                DeliveryFee = orderDto.DeliveryFee,
                Tax = orderDto.Tax,
                Discount = orderDto.Discount,
                CustomerNotes = orderDto.CustomerNotes,
            };

            foreach (var item in orderDto.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId)
                    ?? throw new NotFoundException("Product", item.ProductId);

                order.AddItem(product, item.Quantity);
            }

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

        public async Task<OrderDto> CancelOrderAsync(int orderId, string cancellationReason, bool isCustomer = false)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            order.Cancel(cancellationReason);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> AddOrderItemAsync(int orderId, CreateOrderDetailDto itemDto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(itemDto.ProductId)
                ?? throw new NotFoundException("Product", itemDto.ProductId);

            order.AddItem(product, itemDto.Quantity);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }
        public async Task<OrderDto> RemoveOrderItemAsync(int orderId, int productId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
        ?? throw new NotFoundException("Order", orderId);

            order.RemoveItem(productId);

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

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
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
            var orders = await _unitOfWork.OrderRepository.GetOrdersByDateRangeAsync(startDate, endDate) ?? [];
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

        public async Task<OrderDto> MarkAsPreparingAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            order.TransitionTo(OrderStatus.Preparing);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> MarkAsPreparedAsync(int orderId)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            order.TransitionTo(OrderStatus.Ready);

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        private static readonly Dictionary<string, OrderStatus[]> _roleVisibleStatuses = new()
        {
            [AppRoles.Chef] = new[] { OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Ready },
        };
    }
}
