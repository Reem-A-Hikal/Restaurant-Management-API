using Rest.API.Models.Enums;

namespace Rest.API.Dtos.OrderDtos
{
    public class UpdateOrderDto
    {
        public OrderStatus? Status { get; set; }
        public string PaymentStatus { get; set; }
        public string DeliveryPersonId { get; set; }
        public string Notes { get; set; }
    }
}
