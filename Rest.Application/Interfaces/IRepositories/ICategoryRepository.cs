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
        IQueryable<Category> GetAllQueryable();
        IQueryable<Category> GetFilteredCats(string? searchTerm, string? selectedFilter = "All");
    }
}
