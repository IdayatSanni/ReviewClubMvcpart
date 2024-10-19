
using Microsoft.EntityFrameworkCore;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Models;
using System;
using ReviewClubMvcpart.Data;

namespace ReviewClubMvcpart.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // List all categories with their book counts
        public async Task<IEnumerable<CategoryDto>> ListCategoriesWithBookCount()
        {
            var categories = await _context.Categories.ToListAsync();
            var categoryDtos = new List<CategoryDto>();

            foreach (var category in categories)
            {
                categoryDtos.Add(new CategoryDto
                {
                    Id = category.Id,
                    BookCategory = category.BookCategory,
                    BookCount = category.Books.Count
                });
            }

            return categoryDtos;
        }

        public async Task<CategoryDto?> FindCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return null;
            }

            return new CategoryDto
            {
                Id = category.Id,
                BookCategory = category.BookCategory,
                BookCount = category.Books.Count
            };
        }

        // Add a new category
        public async Task<ServiceResponse> AddCategory(CreateCategoryDto createCategoryDto)
        {
            var response = new ServiceResponse();

            // to validate
            if (string.IsNullOrWhiteSpace(createCategoryDto.BookCategory))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Category name cannot be empty.");
                return response;
            }

            var category = new Category
            {
                BookCategory = createCategoryDto.BookCategory
            };

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Created;
                response.CreatedId = category.Id;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("There was an error adding the category.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        // Update an existing category
        public async Task<ServiceResponse> UpdateCategory(UpdateCategory updateCategoryDto)
        {
            var response = new ServiceResponse();

            var category = await _context.Categories.FindAsync(updateCategoryDto.Id);
            if (category == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Category not found");
                return response;
            }

            category.BookCategory = updateCategoryDto.BookCategory;

            try
            {
                _context.Entry(category).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred updating the category.");
            }

            return response;
        }

        // Delete a category by its ID
        public async Task<ServiceResponse> DeleteCategory(int id)
        {
            var response = new ServiceResponse();
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Category cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the category: " + ex.Message);
            }

            return response;
        }

        // Get books associated with a specific category
        public async Task<IEnumerable<BookDto>> GetBooksByCategory(int categoryId)
        {
            var books = await _context.Books
                .Where(b => b.CategoryId == categoryId)
                .ToListAsync();

            var bookDtos = new List<BookDto>();
            foreach (var book in books)
            {
                bookDtos.Add(new BookDto
                {
                    Id = book.Id,
                    BookName = book.BookName,
                    BookAuthor = book.BookAuthor,
                });
            }

            return bookDtos;
        }
    }
}
