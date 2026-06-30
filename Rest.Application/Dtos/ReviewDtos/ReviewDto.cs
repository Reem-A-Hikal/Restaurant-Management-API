namespace Rest.Application.Dtos.ReviewDtos
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }
        public int OrderId { get; set; }
        public string CustomerId { get; set; }
        public string ReviewerName { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public int? DeliveryRating { get; set; }

        public int? FoodRating { get; set; }
        public int? ProductId { get; set; }
    }
}
