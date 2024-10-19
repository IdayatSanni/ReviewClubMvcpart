using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Interfaces
{
    public interface IReviewerService
    {
        Task<IEnumerable<ReviewerDto>> GetAllReviewers();
        Task<ReviewerWithReviewsDto?> GetReviewerById(int reviewerId);

        // Adds a new reviewer
        Task<ServiceResponse> AddReviewer(CreateReviewerDto createReviewerDto);

        // Updates an existing reviewer
        Task<ServiceResponse> UpdateReviewer(int reviewerId, UpdateReviewerDto updateReviewerDto);

        // Deletes a reviewer by ID
        Task<ServiceResponse> DeleteReviewer(int reviewerId);
    }
}
