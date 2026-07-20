using Rest.Application.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rest.Application.Dtos.DashboardDtos
{
    public class DashboardStatsDto
    {
        public int TotalOrdersToday { get; set; }
        public decimal DailyRevenue { get; set; }
        public int PendingOrdersCount { get; set; }
        public int ActiveDeliveriesCount { get; set; }

        public List<OrderStatusCountDto> OrdersByStatus { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; }
        public List<RevenueTrendPointDto> RevenueTrend { get; set; } = new();
        public List<TopDishDto> TopDishes { get; set; } = new();
    }
}
