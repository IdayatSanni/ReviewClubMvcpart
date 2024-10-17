using ReviewClubCms.Dtos;
using ReviewClubCms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewClubCms.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> ListCategories();
        Task<CategoryDto?> FindCategory(int id);
        Task<ServiceResponse> AddCategory(CategoryDto categoryDto);
        Task<ServiceResponse> UpdateCategory(int id, CategoryDto categoryDto);
        Task<ServiceResponse> DeleteCategory(int id);
    }
}
