using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Rest.Domain.Entities.Enums;

namespace Rest.Domain.Entities
{
    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }

        public DateTime StatusChangeTime { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public DeliveryStatus Status { get; set; }

        public DateTime? DeliveryStartTime { get; set; }

        public DateTime? DeliveryEndTime { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Foreign keys
        [Required]
        public string DeliveryPersonId { get; set; }

        [Required]
        public int OrderId { get; set; }

        // Navigation properties
        public virtual User? DeliveryPerson { get; set; }

        public virtual Order? Order { get; set; }
    }
}
