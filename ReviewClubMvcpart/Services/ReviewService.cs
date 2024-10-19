using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewClubMvcpart.Data;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all reviews with reviewer names and book information
        public async Task<IEnumerable<ReviewDto>> ListAllReviews()
        {
            return await _context.Reviews
                .Include(r => r.Reviewer) // Include the Reviewer
                .Include(r => r.Book)     // Include the Book
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    ReviewText = r.ReviewText,
                    ReviewDate = r.ReviewDate,
                    BookId = r.BookId,
                    ReviewersId = r.ReviewersId,
                    BookName = r.Book != null ? r.Book.BookName : "Unknown", // Handle null Book
                    ReviewerName = r.Reviewer != null ? r.Reviewer.ReviewerName : "Unknown" // Handle null Reviewer
                })
                .ToListAsync();
        }

        // Get a specific review by its ID
        public async Task<ReviewDto?> GetReviewById(int reviewId)
        {
            var review = await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);

            if (review == null) return null;

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                ReviewText = review.ReviewText,
                ReviewDate = review.ReviewDate,
                BookId = review.BookId,
                ReviewersId = review.ReviewersId,
                BookName = review.Book != null ? review.Book.BookName : "Unknown", // Handle null Book
                ReviewerName = review.Reviewer != null ? review.Reviewer.ReviewerName : "Unknown" // Handle null Reviewer
            };
        }

        // Add a new review
        public async Task<ServiceResponse> AddReview(CreateReviewDto createReviewDto)
        {
            var response = new ServiceResponse();

            // Check if the book exists
            var book = await _context.Books.Include(b => b.Reviews).FirstOrDefaultAsync(b => b.Id == createReviewDto.BookId);
            if (book == null)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Book not found.");
                return response;
            }

            // Check if the reviewer exists
            var reviewer = await _context.Reviewers.FindAsync(createReviewDto.ReviewersId);
            if (reviewer == null)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Reviewer not found.");
                return response;
            }

            // Create the review
            var review = new Review
            {
                BookId = book.Id, // Assign the BookId directly
                ReviewersId = reviewer.ReviewersId, // Assign the ReviewersId directly
                ReviewText = createReviewDto.ReviewText,
                ReviewDate = DateTime.UtcNow // Set to current time or manage as needed
            };

            try
            {
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = review.ReviewId; // Return the created review ID
            }
            catch (DbUpdateException ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error occurred while adding the review.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Update an existing review
        public async Task<ServiceResponse> UpdateReview(int reviewId, UpdateReviewDto updateReviewDto)
        {
            var response = new ServiceResponse();

            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Review not found");
                return response;
            }

            // Validation check
            if (string.IsNullOrWhiteSpace(updateReviewDto.ReviewText))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Review text cannot be empty.");
                return response;
            }

            review.ReviewText = updateReviewDto.ReviewText;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred updating the review.");
            }

            return response;
        }

        // List reviews for a specific book
        public async Task<IEnumerable<ReviewDto>> ListReviewsForBook(int bookId)
        {
            return await _context.Reviews
                .Where(review => review.BookId == bookId)
                .Select(review => new ReviewDto
                {
                    ReviewId = review.ReviewId,
                    ReviewText = review.ReviewText,
                    ReviewDate = review.ReviewDate,
                    BookId = review.BookId,
                    ReviewersId = review.ReviewersId,
                    ReviewerName = review.Reviewer != null ? review.Reviewer.ReviewerName : "Unknown" // Handle null Reviewer
                })
                .ToListAsync();
        }

        // Delete a review by its ID
        public async Task<ServiceResponse> DeleteReview(int reviewId)
        {
            var response = new ServiceResponse();

            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Review not found");
                return response;
            }

            try
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the review.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Approve a review by its ID
        public async Task<ServiceResponse> ApproveReview(int reviewId)
        {
            var response = new ServiceResponse();

            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Review not found");
                return response;
            }

            review.IsApproved = true; // Assuming there's an IsApproved property
            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred updating the review approval.");
            }

            return response;
        }
    }
}
