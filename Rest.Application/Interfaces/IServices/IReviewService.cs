using Rest.Application.Dtos.ReviewDtos;
using Rest.Application.Utilities;
using Rest.Domain.Entities;

namespace Rest.Application.Interfaces.IServices
{
    public interface IReviewService
    {
        /// <summary>
        /// Get review for a specific order
        /// </summary>
        Task<ReviewDto?> GetReviewByOrderIdAsync(int OrderId);

        /// <summary>
        /// Get all reviews by a specific customer
        /// </summary>
        Task<IEnumerable<ReviewDto>> GetReviewsByCustomerIdAsync(string customerId);

        Task<PaginatedList<ReviewDto>> GetPaginatedReviewsByProductIdAsync(int ProductId, int pageIndex, int pageSize);

        /// <summary>
        /// Customer creates a review — only after Order is Delivered
        /// and no existing review for this order
        /// </summary>
        Task<ReviewDto> CreateReviewAsync(int orderId, string customerId, CreateReviewDto dto);
        
        /// <summary>
        /// Customer updates their own review
        /// </summary>
        Task<ReviewDto> UpdateReviewAsync(int reviewId, string customerId, UpdateReviewDto dto);

        /// <summary>
        /// Admin deletes a review
        /// </summary>
        Task DeleteReviewAsync(int reviewId);
    }
}
