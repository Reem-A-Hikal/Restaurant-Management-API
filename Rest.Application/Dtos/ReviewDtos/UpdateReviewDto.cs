
using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.ReviewDtos
{
    public class UpdateReviewDto
    {
        [Range(1, 5)]
        public int? Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        [Range(1, 5)]
        public int? DeliveryRating { get; set; }

        [Range(1, 5)]
        public int? FoodRating { get; set; }
    }
}
