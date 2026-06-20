using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    /// <summary>
    /// Interface for product-specific repository operations
    /// </summary>
    public interface IProductRepository :IRepository<Product>
    {
        //IQueryable<Product> GetAllQueryable();
        Task<PaginatedList<Product>> GetPaginatedAsync(int pageIndex, int pageSize, string? searchTerm, string? filter);
        //IQueryable<Product> GetFilteredProducts(string? searchTerm, string? selectedFilter = "All");
        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="categoryId">Category ID to filter by</param>
        /// <returns>List of products in the specified category</returns>
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        /// <summary>
        /// Gets products within a specified price range
        /// </summary>
        /// <param name="minPrice"> Minimum price</param>
        /// <param name="maxPrice"> Maximum price</param>
        /// <returns> List of products within the specified price range</returns>
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> GetAllWithCatAsync();
    }
}
