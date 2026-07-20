using Microsoft.EntityFrameworkCore;
using Rest.Application.Dtos.DashboardDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Implementations.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<Order> _repository;
        public OrderRepository(RestDbContext context,
            IRepository<Order> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task AddAsync(Order entity) => await _repository.AddAsync(entity);

        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        public void Update(Order entity) => _repository.Update(entity);

        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();

        /// <summary>
        /// Lightweight fetch — no Includes.
        /// </summary>
        public async Task<Order?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include (o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order?> GetByIdFullAsync(int id)
        {
            return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.DeliveryAddress)
                    .Include(o => o.ConfirmedBy)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                    .Include(o => o.Payments)
                    .Include(o => o.Deliveries)
                        .ThenInclude(o => o.DeliveryPerson)
                    .Include(o => o.Review)
                    .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusesAsync(IEnumerable<OrderStatus> statuses)
        {
            return await _context.Orders
                .Where(o => statuses.Contains(o.Status))
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            return await _context.Orders
                .Where(o => o.UserId == customerId)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
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

        public async Task<IEnumerable<Order>> GetKitchenQueueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Preparing)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderBy(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .CountAsync(o => o.Status == status);
        }

        public async Task<decimal> GetDailyRevenueAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Orders
                .Where(o => o.OrderDate >= startDate &&
                            o.OrderDate < endDate)
                .Where(o => o.Payments.Any(p => p.Status == PaymentStatus.Completed))
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<Dictionary<OrderStatus, int>> GetOrderCountsByStatusAsync()
        {
            return await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count()})
                .ToDictionaryAsync(x => x.Status, x => x.Count);
        }

        public async Task<List<RevenueTrendPointDto>> GetRevenueTrendAsync(int days)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-(days - 1));
            var today = DateTime.UtcNow.Date;

            var grouped = await _context.Orders
                .Where(o => o.OrderDate >= startDate)
                .Where(o => o.Payments.Any(p => p.Status == PaymentStatus.Completed))
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount) })
                .ToListAsync();

            var result = new List<RevenueTrendPointDto>();
            for (var date = startDate; date<= today; date = date.AddDays(1))
            {
                var match = grouped.FirstOrDefault(g => g.Date == date);
                result.Add(new RevenueTrendPointDto
                {
                    Date = date,
                    Revenue = match?.Revenue ?? 0
                });
            }
            return result;
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count)
        {
            return await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .Select(o => new RecentOrderDto
                {
                    OrderNumber = o.OrderNumber,
                    StatusDisplay = o.Status.ToString(),
                    CustomerName = o.User.FullName,
                    TotalAmount = o.TotalAmount
                }).ToListAsync();
        }
    }
}
