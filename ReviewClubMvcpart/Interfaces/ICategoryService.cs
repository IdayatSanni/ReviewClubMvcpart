using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> ListCategoriesWithBookCount();
        Task<IEnumerable<BookDto>> GetBooksByCategory(int categoryId);
        Task<CategoryDto?> FindCategory(int id);
        Task<ServiceResponse> AddCategory(CreateCategoryDto createCategoryDto); 
        Task<ServiceResponse> UpdateCategory(UpdateCategory updateCategoryDto);
        Task<ServiceResponse> DeleteCategory(int id);
    }
}
