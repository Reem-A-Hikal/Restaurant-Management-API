using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    /// <summary>
    /// Repository interface for category operations
    /// </summary>
    public interface ICategoryRepository :IRepository<Category>
    {
        /// <summary>
        ///  Gets all categories with their products
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Category>> GetAllWithProductsAsync();
        Task<PaginatedList<Category>> GetPaginatedAsync(int pageIndex, int pageSize, string? searchTerm, string? selectedFilter);

        /// <summary>
        /// Active categories with their available products only;
        /// empty categories are left out.
        /// </summary>
        Task<List<Category>> GetMenuAsync();
    }
}
