﻿using Rest.API.Models;

namespace Rest.API.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review?> GetReviewByIdAsync(int id);
        Task AddReviewAsync(Review review);
        Task UpdateReviewAsync(Review review);
        Task DeleteReviewAsync(int id);
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetReviewsByCustomerIdAsync(string customerId);
    }
}
