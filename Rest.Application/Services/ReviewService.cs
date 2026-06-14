using AutoMapper;
using Rest.Application.Dtos.ReviewDtos;
using Rest.Application.Interfaces.IRepositories;
using Rest.Application.Interfaces.IServices;
using Rest.Domain.Entities;
using Rest.Domain.Exceptions;

namespace Rest.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository,  IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id)
                ?? throw new NotFoundException("Review", id);

            return _mapper.Map<ReviewDto>(review);
        }

        public async Task AddReviewAsync(ReviewDto dto)
        {
            if (dto == null)
                throw new ValidationException("Review data is required");
            var review = _mapper.Map<Review>(dto);
            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
        }

        public async Task UpdateReviewAsync(ReviewDto review, int id)
        {
            var existingReview = await _reviewRepository.GetByIdAsync(id);
            _reviewRepository.Update(existingReview);
        }

        public async Task DeleteReviewAsync(int id)
        {
            await _reviewRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepository.GetReviewsByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByCustomerIdAsync(string customerId)
        {
            var reviews = await _reviewRepository.GetReviewsByCustomerIdAsync(customerId);
            return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
    }
}

