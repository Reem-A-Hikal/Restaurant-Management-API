using Rest.API.Models.Enums;

namespace Rest.API.Dtos.OrderDtos
{
    public class OrderStatusCountDto
    {
        public OrderStatus Status { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
