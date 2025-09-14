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
        /// Category Description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Is the category active?
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Display order of the category
        /// </summary>
        public int? DisplayOrder { get; set; }
    }
}
