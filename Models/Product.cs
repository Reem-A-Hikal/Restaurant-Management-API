using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rest.API.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Description { get; set; } //Ingredients, preparation method.

        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
        [Column(TypeName = "nvarchar(255)")]
        public string? Image { get; set; }

        [Required(ErrorMessage = "Preparation time is required")]
        public int PreparationTime { get; set; } // In minutes

        [Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10000")]
        public int? Calories { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        public bool IsAvailable { get; set; } = true;

        [Required]
        public bool IsPromoted { get; set; } = false;

        [Column(TypeName = "decimal(5, 2)")]
        [Range(0.00, 100.00, ErrorMessage = "Discount percent must be between 0 and 100")]
        public decimal DiscountPercent { get; set; } = 0.00m; // Default to 0%

        [Column(TypeName = "decimal(5, 2)")]
        [Range(0.00, 100.00, ErrorMessage = "Allowed discount percent must be between 0 and 100")]
        public decimal AllowedDiscountPercent { get; set; }

        // Foreign key
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public decimal GetDiscountedPrice()
        {
            var discountToApply = Math.Min(DiscountPercent, AllowedDiscountPercent);
            return Price * (100 - discountToApply) / 100;
        }
    }
}
