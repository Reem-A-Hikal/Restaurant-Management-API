using Rest.Application.Dtos.ProductDtos;
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.MenuDtos
{
    public class MenuCategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameAr { get; set; }
        public CategoryStatus Status { get; set; }
        public int DisplayOrder { get; set; }
        public int TotalItems => Products.Count;
        public List<ProductDto> Products { get; set; } = new();
    }
}
