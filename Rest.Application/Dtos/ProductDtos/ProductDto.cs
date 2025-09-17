using Rest.Application.Dtos.CategoryDtos;

namespace Rest.Application.Dtos.ProductDtos
{
    /// <summary>
    /// Data Transfer Object for a product
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// The unique identifier for the product
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// The unique identifier for the product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The price of the product
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The description of the product
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The image URL of the product
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// The preparation time for the product in minutes
        /// </summary>
        public int PreparationTime { get; set; }

        /// <summary>
        /// The number of calories in the product
        /// </summary>
        public int? Calories { get; set; }

        /// <summary>
        ///  Indicates if the product is available
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Indicates if the product is promoted
        /// </summary>
        public bool IsPromoted { get; set; } = false;

        /// <summary>
        /// Discount percentage for the product
        /// </summary>
        public decimal DiscountPercent { get; set; }

        /// <summary>
        /// Maximum allowed discount percentage for the product
        /// </summary>
        public decimal AllowedDiscountPercent { get; set; }

        /// <summary>
        /// The ID of the category to which the product belongs
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The category to which the product belongs
        /// </summary>
        public SimpleCategoryDto? Category { get; set; }
    }
}
