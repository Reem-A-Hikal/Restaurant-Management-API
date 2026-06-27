using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.DeliveryDtos
{
    public class DeliveryDto
    {
        public int DeliveryId { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }

        public DeliveryStatus Status { get; set; }

        public string StatusDisplay => Status.ToString();

        public string DeliveryPersonId { get; set; }
        public string DeliveryPersonName { get; set; }
        public DateTime StatusChangeTime { get; set; }
        public DateTime? DeliveryStartTime { get; set; }
        public DateTime? DeliveryEndTime { get; set; }
        public string? Notes { get; set; }

        public string CustomerAddress { get; set; }

    }
}
