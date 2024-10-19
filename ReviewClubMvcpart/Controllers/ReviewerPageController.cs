using Microsoft.AspNetCore.Mvc;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Models.ViewModels;
using ReviewClubMvcpart.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Controllers
{
    public class ReviewerPageController : Controller
    {
        private readonly IReviewerService _reviewerService;

        // Dependency injection of service interfaces
        public ReviewerPageController(IReviewerService reviewerService)
        {
            _reviewerService = reviewerService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ReviewerPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<ReviewerDto> reviewerDtos = await _reviewerService.GetAllReviewers();
            return View(reviewerDtos);
        }

        // GET: ReviewerPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ReviewerWithReviewsDto? reviewerDto = await _reviewerService.GetReviewerById(id);

            if (reviewerDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new() { "Could not find Reviewer" } });
            }
            else
            {
                return View(reviewerDto);
            }
        }

        // GET: ReviewerPage/New
        public IActionResult New()
        {
            return View();
        }

        // POST: ReviewerPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(CreateReviewerDto createReviewerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResponse response = await _reviewerService.AddReviewer(createReviewerDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        
        // GET: ReviewerPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ReviewerWithReviewsDto? reviewerWithReviews = await _reviewerService.GetReviewerById(id);
            if (reviewerWithReviews == null)
            {
                return View("Error");
            }
            else
            {
                // Directly use properties from ReviewerWithReviewsDto for editing
                var editReviewerDto = new ReviewerDto
                {
                    ReviewerId = reviewerWithReviews.ReviewerId,
                    ReviewerName = reviewerWithReviews.ReviewerName,
                    ReviewerEmail = reviewerWithReviews.ReviewerEmail
                    // Include any other properties if needed
                };

                return View(editReviewerDto);
            }
        }


        // POST: ReviewerPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateReviewerDto updateReviewerDto)
        {
            ServiceResponse response = await _reviewerService.UpdateReviewer(id, updateReviewerDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: ReviewerPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ReviewerWithReviewsDto? reviewerDto = await _reviewerService.GetReviewerById(id);
            if (reviewerDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(reviewerDto);
            }
        }

        // POST: ReviewerPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _reviewerService.DeleteReviewer(id);

            if (response.Status == ServiceResponse.ServiceStatus.Deleted)
            {
                return RedirectToAction("List");
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }
    }
}
