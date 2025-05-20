namespace Rest.API.Dtos.ProductDtos
{
    public class ProductCreateDto
    {
        /// <summary>
        /// Name of the product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Price of the product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Description of the product
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Image URL of the product
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Preparation time for the product in minutes
        /// </summary>
        public int PreparationTime { get; set; }

        /// <summary>
        /// Calories in the product
        /// </summary>
        public int? Calories { get; set; }

        /// <summary>
        /// Indicates if the product is available
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Indicates if the product is promoted
        /// </summary>
        public bool IsPromoted { get; set; } = false;

        /// <summary>
        /// Discount percentage for the product
        /// </summary>
        public decimal DiscountPercent { get; set; } = 0.00m;

        /// <summary>
        /// Maximum allowed discount percentage for the product
        /// </summary>
        public decimal AllowedDiscountPercent { get; set; }

        /// <summary>
        /// Category ID of the product
        /// </summary>
        public int CategoryId { get; set; }
    }
}
