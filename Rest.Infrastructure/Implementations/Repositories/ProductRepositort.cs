using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Domain.Entities;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for product operations
    /// </summary>
    public class ProductRepositort : IProductRepository
    {
        private readonly RestDbContext context;
        private readonly IRepository<Product> repository;

        /// <summary>
        /// Constructor for ProductRepositort
        /// </summary>
        /// <param name="context"> Database context</param>
        /// <param name="repository"> Generic repository</param>
        public ProductRepositort(RestDbContext context, IRepository<Product> repository)
        {
            this.context = context;
            this.repository = repository;
        }
        public IQueryable<Product> GetAllQueryable()
        {
            return context.Products.AsNoTracking().AsQueryable();
        }

        public IQueryable<Product> GetFilteredProducts(string? searchTerm, string? selectedFilter = "All")
        {
            var query = GetAllQueryable();
            if(!string.IsNullOrEmpty(selectedFilter) && selectedFilter != "All")
            {
                query = selectedFilter switch
                {
                    "Available" => query = query.Where(p => p.IsAvailable),
                    "Unavailable" => query = query.Where(p => !p.IsAvailable),
                    _ => query
                };
            }
            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }
            return query.OrderByDescending(p => p.ProductId);
        }


        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="categoryId"> Category ID to filter by</param>
        /// <returns> List of products in the specified category</returns>
        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            var products = await context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();

            return products;
        }

        /// <summary>
        /// Gets products within a specified price range
        /// </summary>
        /// <param name="minPrice"> Minimum price</param>
        /// <param name="maxPrice"> Maximum price</param>
        /// <returns> List of products within the specified price range</returns>
        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all products with their categories
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetAllWithCatAsync()
        {
            return await context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task SaveChangesAsync() => await repository.SaveChangesAsync();

        /// <summary>
        /// Gets a product by its ID
        /// </summary>
        /// <param name="id"> Product ID</param>
        /// <returns> Product with the specified ID</returns>
        public async Task<Product> GetByIdAsync(int id) => await repository.GetByIdAsync(id);

        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="entity"> Product entity to add</param>
        public void Update(Product entity) => repository.Update(entity);

        /// <summary>
        /// Adds a new product asynchronously
        /// </summary>
        /// <param name="entity"> Product entity to add</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task AddAsync(Product entity) => await repository.AddAsync(entity);

        /// <summary>
        /// Deletes a product by its ID
        /// </summary>
        /// <param name="id"> Product ID</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task DeleteAsync(int id) => await repository.DeleteAsync(id);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns> List of all products</returns>
        public async Task<IEnumerable<Product>> GetAllAsync() => await repository.GetAllAsync();

        #region old methods
        ///// <summary>
        ///// Searches products by name
        ///// </summary>
        ///// <param name="searchTerm"> Search term to look for in product names</param>
        ///// <returns> List of matching products</returns>
        //public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        //{
        //    return await context.Products
        //        .Where(p => p.Name.Contains(searchTerm))
        //        .ToListAsync();
        //}

        ///// <summary>
        ///// Gets available products
        ///// </summary>
        ///// <returns> List of available products</returns>
        //public async Task<IEnumerable<Product>> GetAvailableProductsAsync()
        //{
        //    return await context.Products
        //        .Where(p => p.IsAvailable)
        //        .ToListAsync();
        //}
        #endregion
    }
}
