using Microsoft.AspNetCore.Mvc;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Models.ViewModels;
using ReviewClubMvcpart.Models;
using ReviewClubMvcpart.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewClubMvcpart.Controllers
{
    public class ReviewPageController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;

        // Dependency injection of service interfaces
        public ReviewPageController(IBookService bookService, ICategoryService categoryService, IReviewService reviewService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _reviewService = reviewService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: ReviewPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<ReviewDto> reviewDtos = await _reviewService.ListAllReviews();
            return View(reviewDtos);
        }

        // GET: ReviewPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ReviewDto? reviewDto = await _reviewService.GetReviewById(id);
            if (reviewDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new() { "Could not find Review" } });
            }
            return View(reviewDto);
        }

        // GET: ReviewPage/New
        public ActionResult New()
        {
            return View();
        }

        // POST: ReviewPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(CreateReviewDto reviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResponse response = await _reviewService.AddReview(reviewDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { reviewId = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: ReviewPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            ReviewDto? reviewDto = await _reviewService.GetReviewById(id);
            if (reviewDto == null)
            {
                return View("Error");
            }
            return View(reviewDto);
        }

        // POST: ReviewPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateReviewDto reviewDto)
        {
            ServiceResponse response = await _reviewService.UpdateReview(id, reviewDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: ReviewPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            ReviewDto? reviewDto = await _reviewService.GetReviewById(id);
            if (reviewDto == null)
            {
                return View("Error");
            }
            return View(reviewDto);
        }

        // POST: ReviewPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _reviewService.DeleteReview(id);

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
