
using Rest.Domain.Entities.Enums;
using System.ComponentModel.DataAnnotations;

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
        [Required]
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
