using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Interfaces
{
    public interface IBookService
    {
        // This lists the books including their reviews and category
        Task<IEnumerable<BookDto>> ListBooks();

        // This looks for a book if id is provided
        Task<BookDto?> FindBook(int id);

        // This adds a book and it returns service response
        Task<ServiceResponse> AddBook(CreateBookDto createBookDto);

        // This updates a book and returns a service response
        Task<ServiceResponse> UpdateBook(UpdateBookDto updateBookDto); 

        // This deletes a book of the id provided and a service response
        Task<ServiceResponse> DeleteBook(int id);

        // This uploads an image for the book
        Task<ServiceResponse> UploadBookImage(int id, IFormFile bookImage);
    }
}
