using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rest.API.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be positive")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }

        [StringLength(500)]
        public string? SpecialInstructions { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }


        public void CalculateSubtotal()
        {
            Subtotal = Quantity * UnitPrice;
        }
    }
}
