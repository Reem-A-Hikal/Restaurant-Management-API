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

        private IQueryable<Order> Orders() => _context.Orders.IgnoreQueryFilters();

        /// Same graph for every OrderDto => same data whatever endpoint you call.
        private IQueryable<Order> OrdersForDto() => Orders()
            .AsSplitQuery()
            .Include(o => o.User)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .Include(o => o.Payments);

        public async Task AddAsync(Order entity) => await _repository.AddAsync(entity);
        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
        public void Update(Order entity) => _repository.Update(entity);
        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();

        /// <summary>
        /// Lightweight fetch — no Includes.
        /// </summary>
        public async Task<Order?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<IEnumerable<Order>> GetAllAsync() =>
            await OrdersForDto().OrderByDescending(o => o.OrderDate).ToListAsync();

        public async Task<Order?> GetByIdWithDetailsAsync(int id) =>
            await OrdersForDto().FirstOrDefaultAsync(o => o.OrderId == id);

        public async Task<Order?> GetByIdFullAsync(int id)
        {
            return await OrdersForDto()
                    .Include(o => o.ConfirmedBy)
                    .Include(o => o.Deliveries)
                        .ThenInclude(o => o.DeliveryPerson)
                    .Include(o => o.Review)
                    .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status) =>
            await OrdersForDto().Where(o => o.Status == status)
                .OrderByDescending(o => o.OrderDate).ToListAsync();

        public async Task<IEnumerable<Order>> GetOrdersByStatusesAsync(IEnumerable<OrderStatus> statuses)
        {
            var list = statuses.ToList();
            return await OrdersForDto().Where(o => list.Contains(o.Status))
                .OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId) =>
            await OrdersForDto().Where(o => o.UserId == customerId)
                .OrderByDescending(o => o.OrderDate).ToListAsync();

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await OrdersForDto().Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                .OrderByDescending(o => o.OrderDate).ToListAsync();

        public async Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await Orders().CountAsync(o => o.OrderDate >= startDate && o.OrderDate < endDate);

        public async Task<IEnumerable<Order>> GetKitchenQueueAsync() =>
           await OrdersForDto()
               .Where(o => o.Status == OrderStatus.Confirmed || o.Status == OrderStatus.Preparing)
               .OrderBy(o => o.OrderDate).ToListAsync();

        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status) =>
            await Orders().CountAsync(o => o.Status == status);

        public async Task<decimal> GetDailyRevenueAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await Orders()
                .SelectMany(o => o.Payments)
                .Where(p => p.Status == PaymentStatus.Completed && p.PaidAt >= start && p.PaidAt < end)
                .SumAsync(p => p.Amount);
        }

        public async Task<Dictionary<OrderStatus, int>> GetOrderCountsByStatusAsync() =>
            await Orders()
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

        public async Task<List<RevenueTrendPointDto>> GetRevenueTrendAsync(int days)
        {
            days = Math.Clamp(days, 1, 90);
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddDays(-(days - 1));

            var grouped = await Orders()
                .SelectMany(o => o.Payments)
                .Where(p => p.Status == PaymentStatus.Completed
                            && p.PaidAt >= startDate
                            && p.PaidAt < today.AddDays(1))
                .GroupBy(p => p.PaidAt!.Value.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(p => p.Amount) })
                .ToListAsync();

            var byDate = grouped.ToDictionary(g => g.Date, g => g.Revenue);
            var result = new List<RevenueTrendPointDto>(days);
            for (var date = startDate; date<= today; date = date.AddDays(1))
            {
                result.Add(new RevenueTrendPointDto
                {
                    Date = date,
                    Revenue = byDate.GetValueOrDefault(date)
                });
            }
            return result;
        }

        public async Task<List<RecentOrderDto>> GetRecentOrdersAsync(int count)
        {
            return await Orders()
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
