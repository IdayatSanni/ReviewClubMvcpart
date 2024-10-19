using Microsoft.AspNetCore.Mvc;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Models.ViewModels;
using ReviewClubMvcpart.Models;
using ReviewClubMvcpart.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewClubMvcpart.Controllers
{
    public class BookPageController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IReviewService _reviewService;

        // Dependency injection of service interfaces
        public BookPageController(IBookService bookService, ICategoryService categoryService, IReviewService reviewService)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _reviewService = reviewService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: BookPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<BookDto> bookDtos = await _bookService.ListBooks();
            return View(bookDtos);
        }

        // GET: BookPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            BookDto? bookDto = await _bookService.FindBook(id);
            IEnumerable<CategoryDto> categories = await _categoryService.ListCategoriesWithBookCount();
            IEnumerable<ReviewDto> reviews = await _reviewService.ListReviewsForBook(id);

            if (bookDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new() { "Could not find Book" } });
            }
            else
            {
                var bookDetails = new BookDetails
                {
                    Book = bookDto,
                    AllCategories = categories,
                    BookReviews = reviews
                };
                return View(bookDetails);
            }
        }

        // GET: BookPage/New
        public ActionResult New()
        {
            return View();
        }

        // POST: BookPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(CreateBookDto bookDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResponse response = await _bookService.AddBook(bookDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: BookPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            BookDto? bookDto = await _bookService.FindBook(id);
            if (bookDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(bookDto);
            }
        }

        // POST: BookPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateBookDto updateBookDto, IFormFile bookImage) // Changed to UpdateBookDto
        {
            // Confirm receipt of book image
            System.Diagnostics.Debug.WriteLine("Book Image " + bookImage?.Length);

            // Handle image upload first
            if (bookImage != null)
            {
                ServiceResponse imageResponse = await _bookService.UploadBookImage(id, bookImage);
                if (imageResponse.Status == ServiceResponse.ServiceStatus.Error)
                {
                    return View("Error", new ErrorViewModel() { Errors = imageResponse.Messages });
                }
            }

            // Update the book details
            updateBookDto.Id = id; // Set the ID for the update
            ServiceResponse response = await _bookService.UpdateBook(updateBookDto); // Updated to use UpdateBookDto

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: BookPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            BookDto? bookDto = await _bookService.FindBook(id);
            if (bookDto == null)
            {
                return View("Error");
            }
            else
            {
                return View(bookDto);
            }
        }

        // POST: BookPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _bookService.DeleteBook(id);

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
