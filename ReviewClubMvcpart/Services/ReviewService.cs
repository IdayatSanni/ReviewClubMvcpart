using Microsoft.EntityFrameworkCore;
using ReviewClubCms.Data;
using ReviewClubCms.Dtos;
using ReviewClubCms.Interfaces;
using ReviewClubCms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewClubCms.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ApplicationDbContext _context;

        public ReviewService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewDto>> ListReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.Reviewers)
                .ToListAsync();

            var reviewDtos = reviews.Select(review => new ReviewDto
            {
                ReviewId = review.ReviewId,
                ReviewText = review.ReviewText,
                ReviewDate = review.ReviewDate,
                BookName = review.Book?.BookName ?? "Unknown",
                ReviewersName = review.Reviewers?.ReviewersName ?? "Anonymous"
            });

            return reviewDtos;
        }

        public async Task<ReviewDto?> FindReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Book)
                .Include(r => r.Reviewers)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null) return null;

            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                ReviewText = review.ReviewText,
                ReviewDate = review.ReviewDate,
                BookName = review.Book?.BookName ?? "Unknown",
                ReviewersName = review.Reviewers?.ReviewersName ?? "Anonymous"
            };
        }

        public async Task<ServiceResponse> AddReview(CreateReviewDto createReviewDto)
        {
            var serviceResponse = new ServiceResponse();

            // Validate reviewer and book existence
            var reviewer = await _context.Reviewers.FindAsync(createReviewDto.ReviewersId);
            if (reviewer == null)
            {
                serviceResponse.Messages.Add("Invalid reviewer ID");
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                return serviceResponse;
            }

            var book = await _context.Books.FindAsync(createReviewDto.BookId);
            if (book == null)
            {
                serviceResponse.Messages.Add("Invalid book ID");
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                return serviceResponse;
            }

            var review = new Review
            {
                ReviewText = createReviewDto.ReviewText,
                ReviewersId = createReviewDto.ReviewersId,
                BookId = createReviewDto.BookId,
                ReviewDate = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = review.ReviewId;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UpdateReview(int id, UpdateReviewDto updateReviewDto)
        {
            var serviceResponse = new ServiceResponse();

            var existingReview = await _context.Reviews.FindAsync(id);
            if (existingReview == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Review not found");
                return serviceResponse;
            }

            existingReview.ReviewText = updateReviewDto.ReviewText;
            _context.Entry(existingReview).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the review");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> DeleteReview(int id)
        {
            var serviceResponse = new ServiceResponse();

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Review not found");
                return serviceResponse;
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

        public async Task<ServiceResponse> ApproveReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return new ServiceResponse { Status = ServiceResponse.ServiceStatus.NotFound };

            review.IsApproved = true;
            await _context.SaveChangesAsync();

            return new ServiceResponse { Status = ServiceResponse.ServiceStatus.Success };
        }

    }
}
