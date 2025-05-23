using Rest.API.Models.Enums;

namespace Rest.API.Dtos.OrderDtos
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
