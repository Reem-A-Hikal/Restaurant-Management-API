using Rest.Application.Validation;
using Rest.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace Rest.Application.Dtos.ProductDtos
{
    /// <summary>
    /// Request DTO for updating a product.
    /// </summary>
    public class ProductUpdateDto
    {
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }

        [LocalImageUrl(ImageUrlRules.Products), StringLength(255)]
        public string? ImageUrl { get; set; }
        public int? PreparationTime { get; set; }
        public int? Calories { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsPromoted { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? AllowedDiscountPercent { get; set; }
        public ProductStatus? Status { get; set; }
    }
}
