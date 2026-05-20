using Rest.Domain.Entities.Enums;

namespace Rest.Application.Dtos.CategoryDtos
{
    /// <summary>
    /// Data Transfer Object for updating a category
    /// </summary>
    public class CategoryUpdateDto
    {
        /// <summary>
        /// Category ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Category Name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Is the category active?
        /// </summary>
        public CategoryStatus? Status { get; set; }

        /// <summary>
        /// Display order of the category
        /// </summary>
        public int? DisplayOrder { get; set; }
    }
}
