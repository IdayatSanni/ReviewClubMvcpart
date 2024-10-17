using ReviewClubCms.Dtos;
using ReviewClubCms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewClubCms.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDto>> ListReviews();
        Task<ReviewDto?> FindReview(int id);
        Task<ServiceResponse> UpdateReview(int id, UpdateReviewDto updateReviewDto);
        Task<ServiceResponse> AddReview(CreateReviewDto createReviewDto);
        Task<ServiceResponse> DeleteReview(int id);
    }
}
