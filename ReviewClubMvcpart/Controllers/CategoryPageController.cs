using Microsoft.AspNetCore.Mvc;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Models.ViewModels;
using ReviewClubMvcpart.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Controllers
{
    public class CategoryPageController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IBookService _bookService;

        // Dependency injection of service interfaces
        public CategoryPageController(ICategoryService categoryService, IBookService bookService)
        {
            _categoryService = categoryService;
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: CategoryPage/List
        public async Task<IActionResult> List()
        {
            IEnumerable<CategoryDto> categoryDtos = await _categoryService.ListCategoriesWithBookCount();
            return View(categoryDtos);
        }

        // GET: CategoryPage/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            CategoryDto? categoryDto = await _categoryService.FindCategory(id);
            IEnumerable<BookDto> books = await _categoryService.GetBooksByCategory(id);

            if (categoryDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new() { "Could not find Category" } });
            }

            // Information which drives a Category page
            var categoryDetails = new CategoryDetails
            {
                Category = categoryDto,
                BooksInCategory = books
            };
            return View(categoryDetails);
        }

        // GET: CategoryPage/New
        public IActionResult New()
        {
            return View();
        }

        // POST: CategoryPage/Add
        [HttpPost]
        public async Task<IActionResult> Add(CreateCategoryDto createCategoryDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResponse response = await _categoryService.AddCategory(createCategoryDto);

            if (response.Status == ServiceResponse.ServiceStatus.Created)
            {
                return RedirectToAction("Details", new { id = response.CreatedId });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: CategoryPage/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            CategoryDto? categoryDto = await _categoryService.FindCategory(id);
            if (categoryDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new() { "Could not find Category" } });
            }
            return View(categoryDto);
        }

        // POST: CategoryPage/Update/{id}
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategory updateCategoryDto)
        {
            if (id != updateCategoryDto.Id)
            {
                return BadRequest("Category ID mismatch.");
            }

            ServiceResponse response = await _categoryService.UpdateCategory(updateCategoryDto);

            if (response.Status == ServiceResponse.ServiceStatus.Updated)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return View("Error", new ErrorViewModel() { Errors = response.Messages });
            }
        }

        // GET: CategoryPage/ConfirmDelete/{id}
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            CategoryDto? categoryDto = await _categoryService.FindCategory(id);
            if (categoryDto == null)
            {
                return View("Error", new ErrorViewModel() { Errors = new() { "Could not find Category" } });
            }
            return View(categoryDto);
        }

        // POST: CategoryPage/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ServiceResponse response = await _categoryService.DeleteCategory(id);

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
