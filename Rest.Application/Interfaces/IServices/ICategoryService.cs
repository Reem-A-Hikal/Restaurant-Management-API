using Rest.Application.Dtos.CategoryDtos;
using Rest.Application.Utilities;

namespace Rest.Application.Interfaces.IServices
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
        Task<IEnumerable<CategoryWithProductsDto>> GetAllAsync();

        Task<PaginatedList<CategoryWithProductsDto>> GetPaginatedCatsWithFilterAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedStatus);
        /// <summary>
        /// Retrieves a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the category with the specified ID.</returns>
        Task<CategoryWithProductsDto> GetByIdAsync(int id);
        /// <summary>
        /// Adds a new category asynchronously.
        /// </summary>
        /// <param name="category">The category to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<CategoryWithProductsDto> AddAsync(CategoryCreateDto category);
        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id"> The ID of the category to update.</param>
        /// <param name="category">The category to update.</param>
        Task UpdateAsync(int id,CategoryUpdateDto category);
        /// <summary>
        /// Archive a category by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the category to archive.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ArchiveAsync(int id);
    }
}
