using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewClubCms.Dtos;
using ReviewClubCms.Models;

namespace ReviewClubCms.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> ListBooks();
        Task<BookDto?> FindBook(int id);
        Task<ServiceResponse> AddBook(BookDto bookDto);
        Task<ServiceResponse> UpdateBook(int id, BookDto bookDto);
        Task<ServiceResponse> DeleteBook(int id);
        Task<ServiceResponse> UploadBookImage(int id, IFormFile bookImage);
    }
}
