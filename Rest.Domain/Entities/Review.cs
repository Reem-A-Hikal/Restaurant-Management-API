
using Rest.Domain.Exceptions;

namespace Rest.Domain.Entities
{
    public class Review
    {
        public int ReviewId { get; private set; }
        public string? ReviewerName { get; private set; }
        public int Rating { get; private set; }
        public string? Comment { get; private set; }
        public DateTime ReviewDate { get; private set; }
        public int? DeliveryRating { get; private set; }
        public int? FoodRating { get; private set; }

        // Foreign keys
        public int OrderId { get; private set; }
        public string CustomerId { get; private set; }
        public int? ProductId { get; private set; }

        // Navigation properties
        public virtual Order? Order { get; set; }
        public virtual User? Customer { get; set; }
        public virtual Product? Product { get; set; }

        public static Review Create(
            int orderId,
            string customerId,
            string? reviewerName,
            int rating,
            string? comment,
            int? deliveryRating,
            int? foodRating,
            int? productId)
        {
            ValidateRating(rating, deliveryRating, foodRating);

            return new Review
            {
                OrderId = orderId,
                CustomerId = customerId,
                ReviewerName = reviewerName,
                Rating = rating,
                Comment = comment,
                DeliveryRating = deliveryRating,
                FoodRating = foodRating,
                ProductId = productId,
                ReviewDate = DateTime.UtcNow
            };
        }

        #region Domain Method
        public void Update(
            int? rating,
            string? comment,
            int? deliveryRating,
            int? foodRating)
        {
            ValidateRating(rating, deliveryRating, foodRating);
            
            Rating = rating.Value;
            DeliveryRating = deliveryRating.Value;
            FoodRating = foodRating.Value;

            if (comment != null)
                Comment = comment;
        }
        #endregion

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

        private static void ValidateRating(int? rating, int? deliveryRating, int? foodRating)
        {
            if (rating.HasValue && (rating < 1 || rating > 5))
                throw new ValidationException("Rating must be between 1 and 5.");

            if (deliveryRating.HasValue && (deliveryRating < 1 || deliveryRating > 5))
                throw new ValidationException("Delivery rating must be between 1 and 5.");

            if (foodRating.HasValue && (foodRating < 1 || foodRating > 5))
                throw new ValidationException("Food rating must be between 1 and 5.");
        }
    }
}
