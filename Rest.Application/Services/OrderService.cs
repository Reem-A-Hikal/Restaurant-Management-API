using AutoMapper;
using Rest.Application.Dtos.OrderDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Services
{
    public class OrderService :IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public OrderService(IUnitOfWork _unitOfWork, IMapper mapper)
        {
            unitOfWork = _unitOfWork;
            _mapper = mapper;
        }

        public async Task<OrderDto> AddOrderItemAsync(int orderId, CreateOrderDetailDto itemDto)
        {
            var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            // Check if order can still be modified (not yet confirmed or in later stages)
            if (order.Status != OrderStatus.New)
                throw new InvalidOperationException("Cannot modify order items after order has been confirmed.");

            var orderDetail = new OrderDetail
            {
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
            };
            //orderDetail.CalculateSubtotal(); // Assuming this method calculates the subtotal based on quantity and unit price

            order.OrderDetails.Add(orderDetail);
            order.SubTotal = order.OrderDetails.Sum(od => od.Subtotal); // Recalculate subtotal from order details
            order.TotalAmount = order.SubTotal + order.DeliveryFee + order.Tax - order.Discount;

            unitOfWork.OrderRepository.Update(order);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }
        public async Task<OrderDto> RemoveOrderItemAsync(int orderId, int productId)
        {
            var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            // Check if order can still be modified
            if (order.Status != OrderStatus.New)
                throw new InvalidOperationException("Cannot modify order items after order has been confirmed.");

            var itemToRemove = order.OrderDetails.FirstOrDefault(od => od.ProductId == productId);

            if (itemToRemove == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found in order.");
            
            order.OrderDetails.Remove(itemToRemove);
            order.SubTotal = order.OrderDetails.Sum(od => od.Subtotal);
            order.TotalAmount = order.SubTotal + order.DeliveryFee + order.Tax - order.Discount; // Recalculate total amount

            unitOfWork.OrderRepository.Update(order);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createorderDto)
        {
            try
            {
                var order = _mapper.Map<Order>(createorderDto);
                order.OrderDate = DateTime.UtcNow;
                order.OrderNumber = GenerateOrderNumber();
                order.Status = OrderStatus.New;
                var customer = await unitOfWork.UserRepository.GetByIdAsync(createorderDto.UserId);
                if (customer == null)
                    throw new Exception("Customer not found");

                order.UserId = customer.Id;
                order.User = customer;

                var deliveryAddress = await unitOfWork.AddressRepository.GetByIdAsync(createorderDto.DeliveryAddressId);
                if (deliveryAddress == null)
                    throw new Exception("Delivery address not found");

                order.DeliveryAddressId = createorderDto.DeliveryAddressId;
                order.DeliveryAddress = deliveryAddress;

                order.DeliveryFee = createorderDto.DeliveryFee;
                order.Tax = createorderDto.Tax;
                order.Discount = createorderDto.Discount;

                if (createorderDto.OrderDetails == null || !createorderDto.OrderDetails.Any())
                    throw new Exception("Order details are empty");

                order.OrderDetails = createorderDto.OrderDetails
                    .GroupBy(od => od.ProductId)
                    .Select(g => new OrderDetail
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(x => x.Quantity),
                        UnitPrice = g.First().UnitPrice
                    }).ToList();

                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await unitOfWork.ProductRepository.GetByIdAsync(orderDetail.ProductId);
                    if (product == null)
                        throw new Exception($"Product with ID {orderDetail.ProductId} not found");

                    orderDetail.Product = product;
                }
                order.SubTotal = order.OrderDetails.Sum(od => od.UnitPrice * od.Quantity);
                order.TotalAmount = order.SubTotal + order.DeliveryFee + order.Tax - order.Discount;

                
                await unitOfWork.OrderRepository.AddAsync(order);
                await unitOfWork.SaveChangesAsync();

                var orderDto = _mapper.Map<OrderDto>(order);
                orderDto.StatusDisplay = order.Status.ToString(); // Set the status display value
                return orderDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in CreateOrderAsync: {ex.Message}", ex);
            }
        }

        public async Task<OrderDto> AssignDeliveryPersonAsync(int orderId, string deliveryPersonId)
        {
            await unitOfWork.OrderRepository.AssignDeliveryPersonAsync(orderId, deliveryPersonId);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            var updatedOrder = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            var orderdto =  _mapper.Map<OrderDto>(updatedOrder);
            orderdto.DeliveryFee = 50; // Example delivery fee
            orderdto.StatusDisplay = updatedOrder.Status.ToString(); // Set the status display value
            orderdto.SubTotal = updatedOrder.SubTotal; // Set the subtotal value
            orderdto.TotalAmount = updatedOrder.SubTotal + orderdto.DeliveryFee + updatedOrder.Tax - updatedOrder.Discount; // Calculate total amount
            return orderdto;
        }

        public async Task<OrderDto> CancelOrderAsync(int orderId, string cancellationReason)
        {
            await unitOfWork.OrderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Canceled);
            var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            order.Notes = $"{order.Notes}\nCancellation Reason: {cancellationReason}";

            unitOfWork.OrderRepository.Update(order);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<OrderDto> ConfirmOrderAsync(int orderId, string confirmedBy, ConfirmOrderDto confirmDto)
        {
            
            var order = await unitOfWork.OrderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Confirmed);
            //order.ConfirmedBy = confirmedBy;
            order.ConfirmationTime = DateTime.UtcNow;

            if (confirmDto.RequiredTime != null)
                order.RequiredTime = confirmDto.RequiredTime.Value;

            if (!string.IsNullOrEmpty(confirmDto.Notes))
                order.Notes = confirmDto.Notes;

            await unitOfWork.OrderRepository.SaveChangesAsync();

            var orderdto =  _mapper.Map<OrderDto>(order);
            orderdto.DeliveryFee = 50;
            orderdto.TotalAmount = orderdto.SubTotal + orderdto.DeliveryFee + orderdto.Tax - orderdto.Discount; // Calculate total amount
            return orderdto;
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {id} not found.");

            await unitOfWork.OrderRepository.DeleteAsync(id);
            await unitOfWork.OrderRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await unitOfWork.OrderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<decimal> GetDailyRevenueAsync(DateTime date)
        {
            return await unitOfWork.OrderRepository.GetDailyRevenueAsync(date);
        }

        public async Task<IEnumerable<OrderDto>> GetKitchenQueueAsync()
        {
            var kitchenStatuses = new[] { OrderStatus.Confirmed, OrderStatus.Preparing };
            var allOrders = await unitOfWork.OrderRepository.GetAllAsync();
            var kitchenOrders = allOrders.Where(o => kitchenStatuses.Contains(o.Status))
                                       .OrderBy(o => o.RequiredTime ?? o.OrderDate);

            return _mapper.Map<IEnumerable<OrderDto>>(kitchenOrders);
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            var order = await unitOfWork.OrderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {id} not found.");
            }
            var orderDto = _mapper.Map<OrderDto>(order);
            orderDto.StatusDisplay = order.Status.ToString(); // Set the status display value
            return orderDto;
        }

        public async Task<OrderDto> MarkAsDeliveredAsync(int orderId)
        {
            await unitOfWork.OrderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Delivered);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            var updatedOrder = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            return _mapper.Map<OrderDto>(updatedOrder);
        }

        public async Task<OrderDto> MarkAsPreparedAsync(int orderId)
        {
            await unitOfWork.OrderRepository.UpdateOrderStatusAsync(orderId, OrderStatus.Ready);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            var updatedOrder = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            return _mapper.Map<OrderDto>(updatedOrder);
        }

        public async Task ProcessPaymentAsync(int orderId, PaymentMethod paymentMethod)
        {
            var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            //order.PaymentMethod = paymentMethod;

            if (paymentMethod == PaymentMethod.Cash)
            {
                order.PaymentStatus = PaymentStatus.Pending; // Cash payment is pending until delivery
            }
            else
            {
                order.PaymentStatus = PaymentStatus.Completed;
                //order.TransactionId = Guid.NewGuid().ToString();
            }

            unitOfWork.OrderRepository.Update(order);
            await unitOfWork.OrderRepository.SaveChangesAsync();
        }

        

        public async Task UpdateOrderAsync(int id, UpdateOrderDto orderDto)
        {
            var existingOrder = await unitOfWork.OrderRepository.GetByIdAsync(id);
            if (existingOrder == null)
            {
                throw new ArgumentException($"Order with ID {id} not found.");
            }

            if (orderDto.Status.HasValue)
                existingOrder.Status = orderDto.Status.Value;

            existingOrder.PaymentStatus = orderDto.PaymentStatus;

            if (!string.IsNullOrEmpty(orderDto.DeliveryPersonId))
                existingOrder.DeliveryPersonId = orderDto.DeliveryPersonId;

            if (!string.IsNullOrEmpty(orderDto.Notes))
                existingOrder.Notes = orderDto.Notes;

            unitOfWork.OrderRepository.Update(existingOrder);
            await unitOfWork.SaveChangesAsync(); // Save changes to the database
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            await unitOfWork.OrderRepository.UpdateOrderStatusAsync(orderId, newStatus);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            var updatedOrder = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            return _mapper.Map<OrderDto>(updatedOrder);
        }

        public async Task<OrderDto> UpdatePaymentStatusAsync(int orderId, PaymentStatus newStatus)
        {
            var order = await unitOfWork.OrderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            order.PaymentStatus = newStatus;
            unitOfWork.OrderRepository.Update(order);
            await unitOfWork.OrderRepository.SaveChangesAsync();

            return _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string customerId)
        {
            var orders = await unitOfWork.OrderRepository.GetOrdersByCustomerAsync(customerId);
            var ordersdtos =  _mapper.Map<IEnumerable<OrderDto>>(orders);
            foreach (var orderDto in ordersdtos)
            {
                var order = await unitOfWork.OrderRepository.GetByIdAsync(orderDto.OrderId);
                if (order != null)
                {
                    orderDto.StatusDisplay = order.Status.ToString(); // Set the status display value
                }
            }
            return ordersdtos;
        }

        //public async Task<IEnumerable<OrderDto>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        //{
        //    var orders = await unitOfWork.orderRepository.GetOrdersByDateRangeAsync(startDate, endDate);
        //    return _mapper.Map<IEnumerable<OrderDto>>(orders);
        //}

        public async Task<IEnumerable<OrderDto>> GetOrdersByDeliveryPersonAsync(string deliveryPersonId)
        {
            var orders = await unitOfWork.OrderRepository.GetOrdersByDeliveryPersonAsync(deliveryPersonId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await unitOfWork.OrderRepository.GetOrdersByStatusAsync(status);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<IEnumerable<OrderDto>> GetPendingDeliveryOrdersAsync()
        {
            var orders = await unitOfWork.OrderRepository.GetPendingDeliveryOrdersAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            return await unitOfWork.OrderRepository.GetOrderCountByStatusAsync(status);
        }
    }
}
