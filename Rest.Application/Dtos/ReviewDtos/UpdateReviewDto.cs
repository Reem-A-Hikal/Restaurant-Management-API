
namespace Rest.Application.Dtos.ReviewDtos
{
    public class UpdateReviewDto
    {
        public int? Rating { get; set; }
        public string? Comment { get; set; }

        public int? DeliveryRating { get; set; }

        public int? FoodRating { get; set; }
    }
}
