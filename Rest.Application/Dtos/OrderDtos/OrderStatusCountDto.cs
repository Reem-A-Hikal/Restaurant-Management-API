
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.OrderDtos
{
    public class OrderStatusCountDto
    {
        public OrderStatus Status { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }
}
