using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;
using ValidationException = Rest.Domain.Exceptions.ValidationException;

namespace Rest.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public string? Description { get; private set; }
        public string? ImageUrl { get; private set; }
        public int PreparationTime { get; private set; }
        public int? Calories { get; private set; }
        public ProductStatus Status { get; private set; } = ProductStatus.Available;
        public bool IsPromoted { get; private set; }
        public decimal DiscountPercent { get; private set; }
        public decimal AllowedDiscountPercent { get; private set; }
        public int CategoryId { get; private set; }

        // Navigation properties
        public virtual Category? Category { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        #region Factory Method
        public static Product Create(
            string name,
            decimal price,
            int preparationTime,
            int categoryId,
            string? description = null,
            string? imageUrl = null,
            int? calories = null,
            decimal allowedDiscountPercent = 0,
            decimal discountPercent = 0,
            bool isPromoted = false)
        {
            ValidateName(name);
            ValidatePrice(price);
            ValidateDiscount(discountPercent, allowedDiscountPercent);

            return new Product
            {
                Name = name,
                Price = price,
                Description = description,
                ImageUrl = imageUrl,
                PreparationTime = preparationTime,
                Calories = calories,
                CategoryId = categoryId,
                AllowedDiscountPercent = allowedDiscountPercent,
                DiscountPercent = discountPercent,
                IsPromoted = isPromoted,
                Status = ProductStatus.Available
            };
        }
        #endregion

        #region Domain Methods
        public void UpdateDetails(
            string? name,
            string? description,
            string? imageUrl,
            int? preparationTime,
            int? calories,
            int? categoryId)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                ValidateName(name);
                Name = name;
            }

            if (description != null) Description = description;
            if (imageUrl != null) ImageUrl = imageUrl;
            if (preparationTime.HasValue) PreparationTime = preparationTime.Value;
            if (calories.HasValue) Calories = calories.Value;
            if (categoryId.HasValue) CategoryId = categoryId.Value;
        }

        public void UpdatePrice(decimal price)
        {
            ValidatePrice(price);
            Price = price;
        }

        public void SetDiscount(decimal discountPercent, decimal? allowedDiscountPercent = null)
        {
            var allowed = allowedDiscountPercent ?? AllowedDiscountPercent;
            ValidateDiscount(discountPercent, allowed);

            AllowedDiscountPercent = allowed;
            DiscountPercent = discountPercent;
        }

        public void Promote() => IsPromoted = true;
        public void Unpromote() => IsPromoted = false;

        public void Activate()
        {
            if (Status == ProductStatus.Archived)
                throw new BusinessException("Cannot activate an archived product.");
            Status = ProductStatus.Available;
        }

        public void Deactivate()
        {
            if (Status == ProductStatus.Archived)
                throw new BusinessException("Cannot deactivate an archived product.");
            Status = ProductStatus.Unavailable;
        }

        public void Archive()
        {
            Status = ProductStatus.Archived;
        }
        #endregion

        public decimal GetDiscountedPrice()
        {
            var discountToApply = Math.Min(DiscountPercent, AllowedDiscountPercent);
            return Price * (100 - discountToApply) / 100;
        }

        // Helper properties
        public bool IsAvailable => Status == ProductStatus.Available;
        public bool IsArchived => Status == ProductStatus.Archived;

        #region Validation
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException("Product name is required.");
            if (name.Length > 100)
                throw new ValidationException("Product name cannot exceed 100 characters.");
        }

        private static void ValidatePrice(decimal price)
        {
            if (price <= 0)
                throw new ValidationException("Price must be greater than zero.");
        }

        private static void ValidateDiscount(decimal discountPercent, decimal allowedDiscountPercent)
        {
            if (discountPercent < 0 || discountPercent > 100)
                throw new ValidationException("Discount percent must be between 0 and 100.");
            if (allowedDiscountPercent < 0 || allowedDiscountPercent > 100)
                throw new ValidationException("Allowed discount percent must be between 0 and 100.");
            if (discountPercent > allowedDiscountPercent)
                throw new ValidationException("Discount percent cannot exceed allowed discount percent.");
        }
        #endregion
    }
}
