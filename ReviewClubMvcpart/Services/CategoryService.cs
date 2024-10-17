using Microsoft.EntityFrameworkCore;
using ReviewClubCms.Data;
using ReviewClubCms.Dtos;
using ReviewClubCms.Interfaces;
using ReviewClubCms.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewClubCms.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> ListCategories()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    BookCategory = c.BookCategory
                }).ToListAsync();
        }

        public async Task<CategoryDto?> FindCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                BookCategory = category.BookCategory
            };
        }

        public async Task<ServiceResponse> AddCategory(CategoryDto categoryDto)
        {
            var serviceResponse = new ServiceResponse();

            var category = new Category
            {
                BookCategory = categoryDto.BookCategory
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = category.Id;

            return serviceResponse;
        }

        public async Task<ServiceResponse> UpdateCategory(int id, CategoryDto categoryDto)
        {
            var serviceResponse = new ServiceResponse();

            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Category not found");
                return serviceResponse;
            }

            existingCategory.BookCategory = categoryDto.BookCategory;

            try
            {
                await _context.SaveChangesAsync();
                serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the category");
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse> DeleteCategory(int id)
        {
            var serviceResponse = new ServiceResponse();

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                serviceResponse.Messages.Add("Category not found");
                return serviceResponse;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }
    }
}
