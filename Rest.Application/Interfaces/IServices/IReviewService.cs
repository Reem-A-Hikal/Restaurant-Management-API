using Rest.Application.Dtos.ReviewDtos;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> GetAllReviewsAsync();
        Task<ReviewDto?> GetReviewByIdAsync(int id);
        Task AddReviewAsync(ReviewDto review);
        Task UpdateReviewAsync(ReviewDto review, int id);
        Task DeleteReviewAsync(int id);
        Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<ReviewDto>> GetReviewsByCustomerIdAsync(string customerId);
    }
}
