using Rest.Application.Dtos.DashboardDtos;
using Rest.Application.Dtos.OrderDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Services
{
    public class DashboardService: IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int trendDays = 7)
        {
            var today = DateTime.UtcNow.Date;

            var todayOrders = await _unitOfWork.OrderRepository.GetOrdersByDateRangeAsync(today, today.AddDays(1));
            var dailyRevenue = await _unitOfWork.OrderRepository.GetDailyRevenueAsync(today);
            var statusCounts = await _unitOfWork.OrderRepository.GetOrderCountsByStatusAsync();
            var activeDeliveries = await _unitOfWork.DeliveryRepository.GetAllActiveDeliveriesAsync();
            var revenueTrend = await _unitOfWork.OrderRepository.GetRevenueTrendAsync(trendDays);
            var topDishes = await _unitOfWork.OrderDetailRepository.GetTopSellingDishesAsync(5);
            var recentOrders = await _unitOfWork.OrderRepository.GetRecentOrdersAsync(4);

            var pendingStatuses = new[] { OrderStatus.New, OrderStatus.Confirmed, OrderStatus.Preparing };
            var pendingCount = statusCounts
                .Where(kv => pendingStatuses.Contains(kv.Key))
                .Sum(kv => kv.Value);

            var totalOrdersAllTime = statusCounts.Sum(kv => kv.Value);

            var ordersByStatus = statusCounts.Select(kv => new OrderStatusCountDto
            {
                Status = kv.Key,
                Count = kv.Value,
                Percentage = totalOrdersAllTime == 0 ? 0 : Math.Round((decimal)kv.Value / totalOrdersAllTime * 100, 1)
            }).ToList();

            return new DashboardStatsDto
            {
                TotalOrdersToday = todayOrders.Count(),
                DailyRevenue = dailyRevenue,
                PendingOrdersCount = pendingCount,
                ActiveDeliveriesCount = activeDeliveries.Count(),
                OrdersByStatus = ordersByStatus,
                RevenueTrend = revenueTrend,
                TopDishes = topDishes,
                RecentOrders = recentOrders,
            };
        }
    }
}
