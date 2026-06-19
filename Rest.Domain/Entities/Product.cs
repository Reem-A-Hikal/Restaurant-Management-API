using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Rest.Domain.Entities.Enums;

namespace Rest.Domain.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; } //Ingredients, preparation method.

        [StringLength(255, ErrorMessage = "Image URL cannot exceed 255 characters")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Preparation time is required")]
        public int PreparationTime { get; set; } // In minutes

        [Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10000")]
        public int? Calories { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.Available;

        [Required]
        public bool IsPromoted { get; set; } = false;

        [Range(0.00, 100, ErrorMessage = "Discount percent must be between 0 and 100")]
        public decimal DiscountPercent { get; set; } = 0; // Default to 0%

        [Range(0.00, 100, ErrorMessage = "Allowed discount percent must be between 0 and 100")]
        public decimal AllowedDiscountPercent { get; set; }

        // Foreign key
        [Required]
        public int CategoryId { get; set; }

        // Navigation properties
        public virtual Category? Category { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        public decimal GetDiscountedPrice()
        {
            var discountToApply = Math.Min(DiscountPercent, AllowedDiscountPercent);
            return Price * (100 - discountToApply) / 100;
        }

        // Helper properties
        public bool IsAvailable => Status == ProductStatus.Available;
        public bool IsArchived => Status == ProductStatus.Archived;
    }
}
