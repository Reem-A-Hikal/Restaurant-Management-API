using Rest.API.Models;

namespace Rest.API.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId);
    }
}
