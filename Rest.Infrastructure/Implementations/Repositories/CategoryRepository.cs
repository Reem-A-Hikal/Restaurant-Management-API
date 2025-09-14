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
        private readonly RestDbContext context;
        private readonly IRepository<Category> categoryRepository;

        /// <summary>
        /// Constructor for the CategoryRepository class.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="restDb"></param>
        public CategoryRepository(IRepository<Category> repository, RestDbContext restDb)
        {
            context = restDb;
            categoryRepository = repository;
        }
        /// <summary>
        /// Gets all categories with their products
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Category>> GetAllWithProductsAsync()
        {
            return await context.Categories.Include(c => c.Products).OrderBy(c => c.DisplayOrder).ToListAsync();
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
            var category = await context.Categories.Include(c => c.Products)
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
