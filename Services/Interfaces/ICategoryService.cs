using Rest.API.Dtos.CategoryDtos;
using Rest.API.Models;

namespace Rest.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for managing categories.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Retrieves all categories asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of categories.</returns>
        Task<IEnumerable<FullCategoryDto>> GetAllAsync();
        /// <summary>
        /// Retrieves a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the category with the specified ID.</returns>
        Task<FullCategoryDto> GetByIdAsync(int id);
        /// <summary>
        /// Adds a new category asynchronously.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<Category> AddAsync(CategoryCreateDto category);
        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="category">The category to update.</param>
        Task Update(int id,CategoryUpdateDto category);
        /// <summary>
        /// Deletes a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteAsync(int id);
        /// <summary>
        /// Saves changes asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task SaveChangesAsync();
    }
}
