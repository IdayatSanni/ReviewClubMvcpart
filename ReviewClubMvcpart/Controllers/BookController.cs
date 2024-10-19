using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Models;

namespace ReviewClubMvcpart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        // Dependency injection of service interfaces
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Returns a list of Books
        /// </summary>
        /// <returns>
        /// 200 OK
        /// [{BookDto},{BookDto},..]
        /// </returns>
        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<BookDto>>> ListBooks()
        {
            IEnumerable<BookDto> bookDtos = await _bookService.ListBooks();
            return Ok(bookDtos);
        }

        /// <summary>
        /// Returns a single Book specified by its {id}
        /// </summary>
        /// <param name="id">The Book id</param>
        /// <returns>
        /// 200 OK
        /// {BookDto}
        /// or
        /// 404 Not Found
        /// </returns>
        [HttpGet("Find/{id}")]
        public async Task<ActionResult<BookDto>> FindBook(int id)
        {
            var book = await _bookService.FindBook(id);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }

        /// <summary>
        /// Updates a Book
        /// </summary>
        /// <param name="id">The ID of the Book to update</param>
        /// <param name="bookDto">The required information to update the Book</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>
        [HttpPut("Update/{id}")]
        public async Task<ActionResult> UpdateBook(int id, [FromForm] UpdateBookDto updateBookDto)
        {
            if (id != updateBookDto.Id)
            {
                return BadRequest("ID mismatch.");
            }

            // Call the service method
            ServiceResponse response = await _bookService.UpdateBook(updateBookDto);

            // Handle response
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
        /// Adds a Book
        /// </summary>
        /// <param name="createBookDto">The required information to add the Book</param>
        /// <returns>
        /// 201 Created
        /// Location: api/Book/Find/{BookId}
        /// {BookDto}
        /// </returns>
        [HttpPost("Add")]
        public async Task<ActionResult<BookDto>> AddBook(CreateBookDto createBookDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ServiceResponse response = await _bookService.AddBook(createBookDto);
            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Created($"api/Book/Find/{response.CreatedId}", createBookDto);
        }

        /// <summary>
        /// Deletes the Book
        /// </summary>
        /// <param name="id">The id of the Book to delete</param>
        /// <returns>
        /// 204 No Content
        /// or
        /// 404 Not Found
        /// </returns>
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            ServiceResponse response = await _bookService.DeleteBook(id);
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

        /// <summary>
        /// Receives a book picture and saves it
        /// </summary>
        /// <param name="id">The book to update an image for</param>
        /// <param name="bookImage">The picture to change to</param>
        /// <returns>
        /// 200 OK
        /// or
        /// 404 NOT FOUND
        /// or 
        /// 500 BAD REQUEST
        /// </returns>
        [HttpPut("UploadBookImage/{id}")]
        public async Task<IActionResult> UploadBookImage(int id, IFormFile bookImage)
        {
            ServiceResponse response = await _bookService.UploadBookImage(id, bookImage);
            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound();
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, response.Messages);
            }

            return Ok();
        }
    }
}
