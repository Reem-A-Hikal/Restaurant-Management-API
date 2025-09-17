using Microsoft.EntityFrameworkCore;
using Rest.Domain.Entities;
using Rest.Domain.Interfaces.IRepositories;
using Rest.Infrastructure.Data;

namespace Rest.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for category operations
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly RestDbContext _context;
        private readonly IRepository<Category> categoryRepository;

        /// <summary>
        /// Constructor for the CategoryRepository class.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="restDb"></param>
        public CategoryRepository(IRepository<Category> repository, RestDbContext context)
        {
            _context = context;
            categoryRepository = repository;
        }

        public IQueryable<Category> GetAllQueryable() =>
            _context.Categories.AsQueryable();


        public IQueryable<Category> GetFilteredCats(string? searchTerm, string? selectedFilter = "All")
        {
            var query = GetAllQueryable();
            if (!string.IsNullOrEmpty(selectedFilter) && selectedFilter != "All")
            {
                if (selectedFilter == "Active")
                {
                    query = query.Where(c => c.IsActive);
                }
                else if (selectedFilter == "Inactive")
                {
                    query = query.Where(c => !c.IsActive);
                }
                query = query.OrderByDescending(c => c.DisplayOrder);
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Name.Contains(searchTerm)).OrderByDescending(c => c.DisplayOrder);
            }
            return query.OrderByDescending(c => c.DisplayOrder);
        }
        /// <summary>
        /// Gets all categories with their products
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetAllWithProductsAsync()
        {
            return await _context.Categories.Include(c => c.Products).OrderBy(c => c.DisplayOrder).ToListAsync();
        }

        /// <summary>
        /// Adds a new category to the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AddAsync(Category entity) => await categoryRepository.AddAsync(entity);

        /// <summary>
        /// Deletes a category by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id) => await categoryRepository.DeleteAsync(id);

        /// <summary>
        /// Gets all categories from the database.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetAllAsync() => await categoryRepository.GetAllAsync();

        /// <summary>
        /// Gets a category by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Category> GetByIdAsync(int id)
        {
            var category = await _context.Categories.Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found");
            }
                return category;
        }

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        /// <returns></returns>
        public async Task SaveChangesAsync() => await categoryRepository.SaveChangesAsync();

        /// <summary>
        /// Updates an existing category in the database.
        /// </summary>
        /// <param name="entity"></param>
        public void Update(Category entity) => categoryRepository.Update(entity);
    }
}
