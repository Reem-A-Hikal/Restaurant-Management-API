using Microsoft.EntityFrameworkCore;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Interfaces.IRepositories;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<Order> _orderrepository;
        public OrderRepository(RestDbContext context,
            IRepository<Order> orderrepository)
        {
            _context = context;
            _orderrepository = orderrepository;
        }

        public async Task AssignDeliveryPersonAsync(int orderId, string deliveryPersonId)
        {
            var order = await _context.Orders
                .Include(o => o.DeliveryPerson)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} not found.");
            }
            order.DeliveryPersonId = deliveryPersonId;
            order.Status = OrderStatus.OutForDelivery; // Update status to OutForDelivery
            order.DeliveryStartTime = DateTime.UtcNow; // Set the delivery start time
            _context.Entry(order).State = EntityState.Modified; // Mark the order as modified
            await _context.SaveChangesAsync(); // Save changes to the database
        }

        public async Task<decimal> GetDailyRevenueAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Orders
                .Where(o => o.OrderDate >= startDate &&
                            o.OrderDate < endDate &&
                            o.PaymentStatus == "paid")
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .CountAsync(o => o.Status == status);
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _context.Orders
                .Where(o => o.UserId == customerId)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDeliveryPersonAsync(string deliveryPersonId)
        {
            return await _context.Orders
                .Where(o => o.DeliveryPersonId == deliveryPersonId)
                .Include(o => o.User)
                .Include(o => o.DeliveryAddress)
                .OrderByDescending(o => o.DeliveryStartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.User)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetPendingDeliveryOrdersAsync()
        {
            var pendingStatuses = new[] { OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.OutForDelivery };

            return await _context.Orders
                .Where(o => pendingStatuses.Contains(o.Status))
                .Include(o => o.User)
                .Include(o => o.DeliveryAddress)
                .OrderBy(o => o.RequiredTime)
                .ToListAsync();
        }
        public async Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderrepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException($"Order with ID {orderId} not found.");
            }
            order.Status = newStatus;
            switch(newStatus)
            {
                case OrderStatus.Confirmed:
                    order.ConfirmationTime = DateTime.UtcNow;
                    break;
                case OrderStatus.Preparing:
                    order.PreparationStartTime = DateTime.UtcNow;
                    break;
                case OrderStatus.Ready:
                    order.DeliveryStartTime = DateTime.UtcNow;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveryEndTime = DateTime.UtcNow;
                    break;
                case OrderStatus.Canceled:
                    order.CancellationTime = DateTime.UtcNow;
                    break;
            }
            _orderrepository.Update(order);
            return order;
        }

        public async Task SaveChangesAsync() => await _orderrepository.SaveChangesAsync();

        public async Task<IEnumerable<Order>> GetAllAsync() => await _orderrepository.GetAllAsync();
        public void Update(Order entity) => _orderrepository.Update(entity);
        public async Task AddAsync(Order entity) => await _orderrepository.AddAsync(entity);
        public async Task DeleteAsync(int id) => await _orderrepository.DeleteAsync(id);

        public async Task<Order> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.DeliveryAddress)
                    .Include(o => o.DeliveryPerson)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

            return order;
        }
    }
}
