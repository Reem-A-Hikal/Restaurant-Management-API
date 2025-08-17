namespace Rest.Application.Dtos.CategoryDtos
{
    /// <summary>
    /// Data Transfer Object for a category
    /// </summary>
    public class CategoryDto
    {
        /// <summary>
        /// The unique identifier for the category
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// The name of the category
        /// </summary>
        public string Name { get; set; }
    }
}
