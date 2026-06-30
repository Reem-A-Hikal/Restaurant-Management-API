using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IRepositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<Review?> GetByOrderIdAsync(int orderId);
        Task<IEnumerable<Review>> GetByCustomerIdAsync(string customerId);

        Task<PaginatedList<Review>> GetPaginatedAsync(int productId, int pageIndex, int pageSize);
        IQueryable<Review> GetByProductIdQueryable(int productId);
    }
}
