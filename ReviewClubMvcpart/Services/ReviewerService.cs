using Microsoft.EntityFrameworkCore;
using ReviewClubCms.Data;
using ReviewClubCms.Dtos;
using ReviewClubCms.Interfaces;
using ReviewClubCms.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewClubCms.Services
{
    public class ReviewerService : IReviewerService
    {
        private readonly ApplicationDbContext _context;

        public ReviewerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReviewerDto>> ListReviewers()
        {
            return await _context.Reviewers
                .Select(r => new ReviewerDto
                {
                    ReviewersId = r.ReviewersId,
                    ReviewersName = r.ReviewersName,
                    ReviewersEmail = r.ReviewersEmail,
                    ReviewedBookCount = r.Reviews.Count()
                }).ToListAsync();
        }

        public async Task<ReviewerDto?> FindReviewer(int id)
        {
            var reviewer = await _context.Reviewers
                .Include(r => r.Reviews)
                .FirstOrDefaultAsync(r => r.ReviewersId == id);

            if (reviewer == null) return null;

            return new ReviewerDto
            {
                ReviewersId = reviewer.ReviewersId,
                ReviewersName = reviewer.ReviewersName,
                ReviewersEmail = reviewer.ReviewersEmail,
                ReviewedBookCount = reviewer.Reviews.Count()
            };
        }

        public async Task<ServiceResponse> AddReviewer(ReviewerDto reviewerDto)
        {
            var serviceResponse = new ServiceResponse();

            var reviewer = new Reviewer
            {
                ReviewersName = reviewerDto.ReviewersName,
                ReviewersEmail = reviewerDto.ReviewersEmail
            };

            _context.Reviewers.Add(reviewer);
            await _context.SaveChangesAsync();

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = reviewer.ReviewersId;

            return serviceResponse;
        }

        public async Task<ServiceResponse> UpdateReviewer(int id, ReviewerDto reviewerDto)
        {
            var serviceResponse = new ServiceResponse();

            var existingReviewer = await _context.Reviewers.FindAsync(id);
            if (existingReviewer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Reviewer not found");
                return serviceResponse;
            }

            existingReviewer.ReviewersName = reviewerDto.ReviewersName;
            existingReviewer.ReviewersEmail = reviewerDto.ReviewersEmail;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the reviewer");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> DeleteReviewer(int id)
        {
            var serviceResponse = new ServiceResponse();

            var reviewer = await _context.Reviewers.FindAsync(id);
            if (reviewer == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Reviewer not found");
                return serviceResponse;
            }

            _context.Reviewers.Remove(reviewer);
            await _context.SaveChangesAsync();

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }
    }
}
