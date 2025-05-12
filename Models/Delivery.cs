using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Rest.API.Models.Enums;

namespace Rest.API.Models
{
    public class Delivery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryId { get; set; }

        public DateTime StatusChangeTime { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public DeliveryStatus Status { get; set; }

        public DateTime? DeliveryStartTime { get; set; }

        public DateTime? DeliveryEndTime { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Longitude { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Foreign keys
        [Required]
        public int DeliveryPersonId { get; set; }

        [Required]
        public int OrderId { get; set; }

        // Navigation properties
        [ForeignKey("DeliveryPersonId")]
        public virtual User? DeliveryPerson { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
