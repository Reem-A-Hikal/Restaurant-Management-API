using AutoMapper;
using Rest.Application.Dtos.ReviewDtos;
using Rest.Application.Interfaces;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Application.Utilities;
using Rest.Domain.Entities;
using Rest.Domain.Entities.Enums;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewService(IUnitOfWork unitOfWork,  IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ReviewDto?> GetReviewByOrderIdAsync(int orderId)
        {
            var review = await _unitOfWork.ReviewRepository.GetByOrderIdAsync(orderId);
            return review == null ? null : _mapper.Map<ReviewDto>(review);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByCustomerIdAsync(string customerId)
        {
            var reviews = await _unitOfWork.ReviewRepository.GetByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<PaginatedList<ReviewDto>> GetPaginatedReviewsByProductIdAsync(int productId, int pageIndex, int pageSize)
        {
            var paginated = await _unitOfWork.ReviewRepository.GetPaginatedAsync(productId, pageIndex, pageSize);
            var mapped = _mapper.Map<List<ReviewDto>>(paginated.Items);
            return new PaginatedList<ReviewDto>(mapped, paginated.TotalItems, paginated.PageIndex, paginated.PageSize);
        }

        public async Task<ReviewDto> CreateReviewAsync(int orderId, string customerId, CreateReviewDto dto)
        {
            var order = await _unitOfWork.OrderRepository.GetByIdWithDetailsAsync(orderId)
                ?? throw new NotFoundException("Order", orderId);

            if (order.UserId != customerId)
                throw new ForbiddenException("You can only review your own orders");

            if (order.Status != OrderStatus.Delivered)
                throw new BusinessException("You can only review a delivered order");

            if (dto.ProductId.HasValue)
            {
                var line = order.OrderDetails.FirstOrDefault(d => d.ProductId == dto.ProductId.Value)
                    ?? throw new ValidationException("The selected product is not part of this order.");

                if (line.Product.Status == ProductStatus.Archived)
                    throw new BusinessException("This product is no longer on the menu and can't be reviewed.");
            }

            if (await _unitOfWork.ReviewRepository.GetByOrderIdAsync(orderId) != null)
                throw new BusinessException("You have already reviewed this order.");

            var review = Review.Create(
                orderId,
                customerId,
                order.User?.FullName,
                dto.Rating,
                dto.Comment,
                dto.DeliveryRating,
                dto.FoodRating,
                dto.ProductId);

            await _unitOfWork.ReviewRepository.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.ReviewRepository.GetByIdAsync(review.ReviewId);
            return _mapper.Map<ReviewDto>(created);
        }

        public async Task<ReviewDto> UpdateReviewAsync(int reviewId, string customerId, UpdateReviewDto dto)
        {
            var review = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId)
                ?? throw new NotFoundException("Review", reviewId);

            if (review.CustomerId != customerId)
                throw new ForbiddenException("You can only update your own reviews");

            review.Update(dto.Rating, dto.Comment, dto.DeliveryRating, dto.FoodRating);

            _unitOfWork.ReviewRepository.Update(review);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ReviewDto>(review);
        }

        public async Task DeleteReviewAsync(int reviewId)
        {
            _ = await _unitOfWork.ReviewRepository.GetByIdAsync(reviewId)
                 ?? throw new NotFoundException("Review", reviewId);

            await _unitOfWork.ReviewRepository.DeleteAsync(reviewId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<RatingSummaryDto> GetProductRatingSummaryAsync(int productId)
        {
            var (average, count) = await _unitOfWork.ReviewRepository.GetProductRatingSummaryAsync(productId);
            return new RatingSummaryDto { Average = average, Count = count };
        }
    }
}

