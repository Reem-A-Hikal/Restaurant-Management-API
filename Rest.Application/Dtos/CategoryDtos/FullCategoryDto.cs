

using Rest.Application.Dtos.ProductDtos;

namespace Rest.Application.Dtos.CategoryDtos
{
    /// <summary>
    /// DTO for category information.
    /// </summary>
    public class FullCategoryDto
    {
        /// <summary>
        /// The unique identifier for the category.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unique identifier for the category.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates whether the category is active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// The display order of the category in the list.
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// A collection of products associated with this category.
        /// </summary>
        public ICollection<SimpleProductDto> Products { get; set; }
    }
}
