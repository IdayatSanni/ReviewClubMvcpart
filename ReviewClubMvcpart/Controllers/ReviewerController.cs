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
    public class ReviewerController : ControllerBase
    {
        private readonly IReviewerService _reviewerService;

        // Dependency injection of the reviewer service
        public ReviewerController(IReviewerService reviewerService)
        {
            _reviewerService = reviewerService;
        }

        /// <summary>
        /// Returns a list of all reviewers with their reviews
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{ReviewerWithReviewsDto},{ReviewerWithReviewsDto},..]
        /// </returns>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<ReviewerDto>>> ListReviewers()
        {
            IEnumerable<ReviewerDto> reviewers = await _reviewerService.GetAllReviewers();
            return Ok(reviewers);
        }

        /// <summary>
        /// Gets a reviewer by ID with their reviews
        /// </summary>
        /// <param name="id">The ID of the reviewer</param>
        /// <returns>
        /// 200 OK
        /// {ReviewerWithReviewsDto}
        /// or
        /// 404 Not Found
        /// </returns>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<ReviewerWithReviewsDto>> GetReviewerById(int id)
        {
            var reviewer = await _reviewerService.GetReviewerById(id);
            if (reviewer == null)
            {
                return NotFound();
            }
            return Ok(reviewer);
        }

        /// <summary>
        /// Adds a new reviewer
        /// </summary>
        /// <param name="createReviewerDto">The required information to add the reviewer</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Reviewer/Find/{ReviewerId}
        /// {ReviewerWithReviewsDto}
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<ReviewerWithReviewsDto>> AddReviewer(CreateReviewerDto createReviewerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResponse response = await _reviewerService.AddReviewer(createReviewerDto);
            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return Created($"api/Reviewer/Find/{response.CreatedId}", createReviewerDto);
            }
            else
            {
                return StatusCode(500, response.Messages);
            }
        }

        /// <summary>
        /// Updates an existing reviewer
        /// </summary>
        /// <param name="id">The ID of the reviewer to update</param>
        /// <param name="updateReviewerDto">The updated reviewer information</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateReviewer(int id, UpdateReviewerDto updateReviewerDto)
        {
            ServiceResponse response = await _reviewerService.UpdateReviewer(id, updateReviewerDto);
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
        /// Deletes a reviewer by ID
        /// </summary>
        /// <param name="id">The ID of the reviewer to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteReviewer(int id)
        {
            ServiceResponse response = await _reviewerService.DeleteReviewer(id);
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
    }
}
