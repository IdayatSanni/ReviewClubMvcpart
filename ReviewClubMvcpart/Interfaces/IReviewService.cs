using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Interfaces
{
    public interface IReviewService
    {
        // List all reviews with reviewer names and book information
        Task<IEnumerable<ReviewDto>> ListAllReviews();

        // Get a specific review by its ID
        Task<ReviewDto?> GetReviewById(int reviewId);

        // Add a new review
        Task<ServiceResponse> AddReview(CreateReviewDto createReviewDto);

        // Update an existing review
        Task<ServiceResponse> UpdateReview(int reviewId, UpdateReviewDto updateReviewDto);

        // List reviews for a specific book
        Task<IEnumerable<ReviewDto>> ListReviewsForBook(int bookId);

        // Delete a review by its ID
        Task<ServiceResponse> DeleteReview(int reviewId);

        // Approve a review by its ID
        Task<ServiceResponse> ApproveReview(int reviewId);
        
    }
}
