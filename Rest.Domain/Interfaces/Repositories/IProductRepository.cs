using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interface for product-specific repository operations
    /// </summary>
    public interface IProductRepository :IRepository<Product>
    {
        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="categoryId">Category ID to filter by</param>
        /// <returns>List of products in the specified category</returns>
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);

        /// <summary>
        /// Gets available products
        /// </summary>
        /// <returns>List of available products</returns>
        Task<IEnumerable<Product>> GetAvailableProductsAsync();

        /// <summary>
        /// Searches products by name
        /// </summary>
        /// <param name="searchTerm">Search term to look for in product names</param>
        /// <returns>List of matching products</returns>
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);

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
