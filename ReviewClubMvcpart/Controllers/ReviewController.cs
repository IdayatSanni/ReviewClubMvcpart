using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        // Dependency injection of the review service
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// Returns a list of all reviews.
        /// </summary>
        /// <returns>200 OK with a list of ReviewDto</returns>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> ListReviews()
        {
            IEnumerable<ReviewDto> reviews = await _reviewService.ListAllReviews();
            return Ok(reviews);
        }

        /// <summary>
        /// Returns a specific review by its ID.
        /// </summary>
        /// <param name="reviewId">The ID of the review</param>
        /// <returns>200 OK with ReviewDto or 404 Not Found</returns>
        [HttpGet("Find/{reviewId}")]
        public async Task<ActionResult<ReviewDto>> GetReviewById(int reviewId)
        {
            var review = await _reviewService.GetReviewById(reviewId);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }

        /// <summary>
        /// Adds a new review.
        /// </summary>
        /// <param name="createReviewDto">The required information to add the review</param>
        /// <returns>201 Created with the location of the new review</returns>
        [HttpPost("Add")]
        public async Task<ActionResult> AddReview(CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResponse response = await _reviewService.AddReview(createReviewDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }
            return CreatedAtAction(nameof(GetReviewById), new { reviewId = response.CreatedId }, createReviewDto);
        }

        /// <summary>
        /// Updates an existing review.
        /// </summary>
        /// <param name="reviewId">The ID of the review to update</param>
        /// <param name="updateReviewDto">The required information to update the review</param>
        /// <returns>204 No Content or appropriate error response</returns>
        [HttpPut("Update/{reviewId}")]
        public async Task<ActionResult> UpdateReview(int reviewId, UpdateReviewDto updateReviewDto)
        {
            ServiceResponse response = await _reviewService.UpdateReview(reviewId, updateReviewDto);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a review by its ID.
        /// </summary>
        /// <param name="reviewId">The ID of the review to delete</param>
        /// <returns>204 No Content or 404 Not Found</returns>
        [HttpDelete("Delete/{reviewId}")]
        public async Task<ActionResult> DeleteReview(int reviewId)
        {
            ServiceResponse response = await _reviewService.DeleteReview(reviewId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }

        /// <summary>
        /// Approves a review by its ID.
        /// </summary>
        /// <param name="reviewId">The ID of the review to approve</param>
        /// <returns>204 No Content or 404 Not Found</returns>
        [HttpPut("Approve/{reviewId}")]
        public async Task<ActionResult> ApproveReview(int reviewId)
        {
            ServiceResponse response = await _reviewService.ApproveReview(reviewId);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(response.Messages);
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return NoContent();
        }
    }
}
