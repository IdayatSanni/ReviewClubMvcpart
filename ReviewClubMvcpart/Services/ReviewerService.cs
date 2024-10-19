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
    public class ReviewerService : IReviewerService
    {
        private readonly ApplicationDbContext _context;

        // Dependency injection of the database context
        public ReviewerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Gets a list of all reviewers
        public async Task<IEnumerable<ReviewerDto>> GetAllReviewers()
        {
            return await _context.Reviewers
                .Select(r => new ReviewerDto
                {
                    ReviewerId = r.ReviewersId,
                    ReviewerName = r.ReviewerName,
                    ReviewerEmail = r.ReviewersEmail,
                    ReviewedBookCount = r.Reviews.Count() // Assuming Reviews is a navigation property
                })
                .ToListAsync();
        }

        // Gets a reviewer by ID with their reviews
        public async Task<ReviewerWithReviewsDto?> GetReviewerById(int reviewerId)
        {
            return await _context.Reviewers
                .Where(r => r.ReviewersId == reviewerId)
                .Select(r => new ReviewerWithReviewsDto
                {
                    ReviewerId = r.ReviewersId,
                    ReviewerName = r.ReviewerName,
                    ReviewerEmail = r.ReviewersEmail,
                    Reviews = r.Reviews.Select(review => new ReviewInfoDto
                    {
                        ReviewId = review.ReviewId,
                        BookName = review.Book.BookName, // Assuming Book has a BookName property
                        ReviewText = review.ReviewText,
                        ReviewDate = review.ReviewDate
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        // Adds a new reviewer
        public async Task<ServiceResponse> AddReviewer(CreateReviewerDto createReviewerDto)
        {
            var response = new ServiceResponse();

            // Validation checks for the incoming DTO
            if (string.IsNullOrWhiteSpace(createReviewerDto.ReviewerName))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Reviewer name cannot be empty.");
                return response;
            }

            if (string.IsNullOrWhiteSpace(createReviewerDto.ReviewerEmail))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Reviewer email cannot be empty.");
                return response;
            }

            var reviewer = new Reviewer
            {
                ReviewerName = createReviewerDto.ReviewerName,
                ReviewersEmail = createReviewerDto.ReviewerEmail
            };

            try
            {
                await _context.Reviewers.AddAsync(reviewer);
                await _context.SaveChangesAsync();

                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = reviewer.ReviewersId;
            }
            catch (DbUpdateException ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("There was an error adding the reviewer.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Updates an existing reviewer
        public async Task<ServiceResponse> UpdateReviewer(int reviewerId, UpdateReviewerDto updateReviewerDto)
        {
            var response = new ServiceResponse();

            var reviewer = await _context.Reviewers.FindAsync(reviewerId);
            if (reviewer == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Reviewer not found");
                return response;
            }

            // Validation checks
            if (string.IsNullOrWhiteSpace(updateReviewerDto.ReviewerName))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Reviewer name cannot be empty.");
                return response;
            }

            if (string.IsNullOrWhiteSpace(updateReviewerDto.ReviewerEmail))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Reviewer email cannot be empty.");
                return response;
            }

            reviewer.ReviewerName = updateReviewerDto.ReviewerName;
            reviewer.ReviewersEmail = updateReviewerDto.ReviewerEmail;

            try
            {
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred updating the reviewer.");
            }

            return response;
        }

        // Deletes a reviewer by ID
        public async Task<ServiceResponse> DeleteReviewer(int reviewerId)
        {
            var response = new ServiceResponse();

            var reviewer = await _context.Reviewers.FindAsync(reviewerId);
            if (reviewer == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Reviewer not found");
                return response;
            }

            try
            {
                _context.Reviewers.Remove(reviewer);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the reviewer");
                response.Messages.Add(ex.Message);
            }

            return response;
        }
    }
}
