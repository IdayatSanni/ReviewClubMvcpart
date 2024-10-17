using ReviewClubCms.Dtos;
using ReviewClubCms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewClubCms.Interfaces
{
    public interface IReviewerService
    {
        Task<IEnumerable<ReviewerDto>> ListReviewers();
        Task<ReviewerDto?> FindReviewer(int id);
        Task<ServiceResponse> AddReviewer(ReviewerDto reviewerDto);
        Task<ServiceResponse> UpdateReviewer(int id, ReviewerDto reviewerDto);
        Task<ServiceResponse> DeleteReviewer(int id);
    }
}
