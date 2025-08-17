using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Rest.Domain.Entities
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReviewId { get; set; }

        [StringLength(50)]
        public string? ReviewerName { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Comment { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        [Range(1, 5, ErrorMessage = "Delivery rating must be between 1 and 5")]
        public int? DeliveryRating { get; set; }

        [Range(1, 5, ErrorMessage = "Food rating must be between 1 and 5")]
        public int? FoodRating { get; set; }

        // Foreign keys
        [Required]
        public int OrderId { get; set; }

        [Required]
        public string CustomerId { get; set; }

        public int? ProductId { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User? Customer { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        public double GetAvgRating()
        {
            double totalRating = 0;
            int count = 0;
            if (DeliveryRating.HasValue)
            {
                totalRating += DeliveryRating.Value;
                count++;
            }
            if (FoodRating.HasValue)
            {
                totalRating += FoodRating.Value;
                count++;
            }
            return count > 0 ? totalRating / count : 0;
        }
    }
}
