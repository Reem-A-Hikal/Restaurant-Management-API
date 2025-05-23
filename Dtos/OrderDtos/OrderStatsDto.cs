namespace Rest.API.Dtos.OrderDtos
{
    public class OrderStatsDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int PreparedOrders { get; set; }
        public int OutForDeliveryOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal DailyRevenue { get; set; }
    }
}
