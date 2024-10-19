using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Returns a list of Categories with book counts.
        /// </summary>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> ListCategories()
        {
            var categoryDtos = await _categoryService.ListCategoriesWithBookCount();
            return Ok(categoryDtos);
        }

        /// <summary>
        /// Returns a list of books in a specified category by its ID.
        /// </summary>
        [HttpGet("{id}/books")]
        public async Task<IActionResult> GetBooksByCategory(int id)
        {
            var books = await _categoryService.GetBooksByCategory(id);
            if (books == null || !books.Any())
            {
                return NotFound("No books found for this category.");
            }
            return Ok(books);
        }

        /// <summary>
        /// Adds a new Category.
        /// </summary>
        [HttpPost("Add")]
        public async Task<ActionResult<CategoryDto>> AddCategory(CreateCategoryDto createCategoryDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _categoryService.AddCategory(createCategoryDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            // Return the created category with its ID
            var createdCategory = await _categoryService.FindCategory(response.CreatedId);
            return CreatedAtAction(nameof(GetBooksByCategory), new { id = response.CreatedId }, createdCategory);
        }

        /// <summary>
        /// Updates an existing Category.
        /// </summary>
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateCategory(int id, UpdateCategory updateCategoryDto)
        {
            if (id != updateCategoryDto.Id)
            {
                return BadRequest("Category ID mismatch.");
            }

            var response = await _categoryService.UpdateCategory(updateCategoryDto);
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
        /// Deletes the specified Category.
        /// </summary>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var response = await _categoryService.DeleteCategory(id);
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
