using Microsoft.EntityFrameworkCore;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Infrastructure.Data;
using Rest.Infrastructure.Helpers;

namespace Rest.Infrastructure.Implementations.Repositories
{
    /// <summary>
    /// Repository implementation for product operations
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<Product> _repository;

        /// <summary>
        /// Constructor for ProductRepositort
        /// </summary>
        /// <param name="context"> Database context</param>
        /// <param name="repository"> Generic repository</param>
        public ProductRepository(RestDbContext context, IRepository<Product> repository)
        {
            _context = context;
            _repository = repository;
        }

        private IQueryable<Product> GetFilteredProducts(string? searchTerm, string? selectedFilter = "All")
        {
            var query = _context.Products.AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(selectedFilter) && selectedFilter != "All")
            {
                query = selectedFilter switch
                {
                    "Available" => query = query.Where(p => p.Status == ProductStatus.Available),
                    "Unavailable" => query = query.Where(p => p.Status == ProductStatus.Unavailable),
                    _ => query
                };
            }
            if(!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm));
            }
            return query.OrderByDescending(p => p.ProductId);
        }

        public async Task<PaginatedList<Product>> GetPaginatedAsync(
                    int pageIndex, int pageSize,
                    string? searchTerm, string? selectedFilter)
        {
            var query = GetFilteredProducts(searchTerm, selectedFilter);
            return await PaginationHelper.CreateAsync(query, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets products by category
        /// </summary>
        /// <param name="categoryId"> Category ID to filter by</param>
        /// <returns> List of products in the specified category</returns>
        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        /// <summary>
        /// Gets products within a specified price range
        /// </summary>
        /// <param name="minPrice"> Minimum price</param>
        /// <param name="maxPrice"> Maximum price</param>
        /// <returns> List of products within the specified price range</returns>
        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
                .ToListAsync();
        }

        /// <summary>
        /// Gets all products with their categories
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetAllWithCatAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        /// <summary>
        /// Saves changes to the database
        /// </summary>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task SaveChangesAsync() => await _repository.SaveChangesAsync();

        /// <summary>
        /// Gets a product by its ID
        /// </summary>
        /// <param name="id"> Product ID</param>
        /// <returns> Product with the specified ID</returns>
        public async Task<Product> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

        /// <summary>
        /// Adds a new product
        /// </summary>
        /// <param name="entity"> Product entity to add</param>
        public void Update(Product entity) => _repository.Update(entity);

        /// <summary>
        /// Adds a new product asynchronously
        /// </summary>
        /// <param name="entity"> Product entity to add</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task AddAsync(Product entity) => await _repository.AddAsync(entity);

        /// <summary>
        /// Deletes a product by its ID
        /// </summary>
        /// <param name="id"> Product ID</param>
        /// <returns> Task representing the asynchronous operation</returns>
        public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns> List of all products</returns>
        public async Task<IEnumerable<Product>> GetAllAsync() => await _repository.GetAllAsync();
    }
}
