using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rest.Domain.Entities
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        // Foreign keys
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1; // Default to 1

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be positive")]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        [StringLength(500)]
        public string? SpecialInstructions { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
