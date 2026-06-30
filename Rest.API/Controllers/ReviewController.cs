using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rest.Application.Dtos.ReviewDtos;
using Rest.Application.Interfaces.IServices;
using System.Security.Claims;

namespace Rest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewController : BaseController
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Customer creates a review for a delivered order
        /// </summary>
        [HttpPost("order/{orderId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateReview(int orderId, [FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.CreateReviewAsync(orderId, currentUserId!, dto);

            return SuccessResponse(result, "Review created successfully");
        }

        /// <summary>
        /// Customer updates their own review
        /// </summary>
        [HttpPut("{reviewId}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] UpdateReviewDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationErrorResponse(
                    ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)));

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewService.UpdateReviewAsync(reviewId, currentUserId!, dto);

            return SuccessResponse(result, "Review updated successfully");
        }

        /// <summary>
        /// Admin deletes a review
        /// </summary>
        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            await _reviewService.DeleteReviewAsync(reviewId);

            return SuccessResponse<string>(null, "Review deleted successfully");
        }

        /// <summary>
        /// Get review for a specific order — Admin or Customer who owns it
        /// </summary>
        [HttpGet("order/{orderId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetReviewByOrder(int orderId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var result = await _reviewService.GetReviewByOrderIdAsync(orderId);
            if (result == null)
                return SuccessResponse(result, "No review found for this order.");

            if (userRole == "Customer" && result.CustomerId != currentUserId)
                return ForbiddenResponse("You can only view your own reviews.");

            return SuccessResponse(result, "Review retrieved successfully");
        }

        /// <summary>
        /// Get paginated reviews for a product — Public
        /// </summary>
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByProduct(
            int productId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _reviewService.GetPaginatedReviewsByProductIdAsync(
                productId, pageIndex, pageSize);

            return SuccessResponse(result, "Reviews retrieved successfully");
        }

        /// <summary>
        /// Get all reviews by a customer — Admin or the Customer himself
        /// </summary>
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetReviewsByCustomer(string customerId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Customer" && customerId != currentUserId)
                return ForbiddenResponse("You can only view your own reviews.");

            var result = await _reviewService.GetReviewsByCustomerIdAsync(customerId);
            return SuccessResponse(result, "Reviews retrieved successfully");
        }
    }
}
