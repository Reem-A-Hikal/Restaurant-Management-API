using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        new Task<IEnumerable<Review>> GetAllAsync();
        new Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId);
    }
}
