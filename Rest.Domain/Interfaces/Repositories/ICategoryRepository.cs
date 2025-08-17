using Rest.Domain.Entities;

namespace Rest.Domain.Interfaces.Repositories
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
    }
}
