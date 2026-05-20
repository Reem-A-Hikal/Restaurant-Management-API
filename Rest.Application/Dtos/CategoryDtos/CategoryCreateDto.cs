
using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.CategoryDtos
{
    /// <summary>
    /// Data Transfer Object for creating a category
    /// </summary>
    public class CategoryCreateDto
    {
        /// <summary>
        /// Category Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is the category active?
        /// </summary>
        public CategoryStatus Status { get; set; } = CategoryStatus.Active;

        /// <summary>
        /// Display order of the category
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
