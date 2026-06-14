using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.DeliveryDtos
{
    public class DeliveryDto
    {
        public int DeliveryId { get; set; }

        public DateTime StatusChangeTime { get; set; } = DateTime.UtcNow;

        public DeliveryStatus Status { get; set; }

        public DateTime? DeliveryStartTime { get; set; }

        public DateTime? DeliveryEndTime { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string? Notes { get; set; }
    }
}
