using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReviewClubCms.Data;
using ReviewClubCms.Dtos;
using ReviewClubCms.Interfaces;
using ReviewClubCms.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReviewClubCms.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a comprehensive list of books with their details and review counts
        public async Task<IEnumerable<BookWithReviewsDto>> ListBooks()
        {
            return await _context.Books
                .Select(b => new BookWithReviewsDto
                {
                    Id = b.Id,
                    BookName = b.BookName,
                    BookAuthor = b.BookAuthor,
                    CategoryId = b.CategoryId,
                    IsBookOfTheMonth = b.IsBookOfTheMonth,
                    BookPicture = b.BookPicture,
                    ReviewCount = b.Reviews.Count() // Get the review count
                })
                .ToListAsync();
        }

        // Finds a specific book by its ID
        public async Task<BookDto?> FindBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            return new BookDto
            {
                Id = book.Id,
                BookName = book.BookName,
                BookAuthor = book.BookAuthor,
                CategoryId = book.CategoryId,
                IsBookOfTheMonth = book.IsBookOfTheMonth
            };
        }

        // Adds a new book
        public async Task<ServiceResponse> AddBook(BookDto bookDto)
        {
            var book = new Book
            {
                BookName = bookDto.BookName,
                BookAuthor = bookDto.BookAuthor,
                CategoryId = bookDto.CategoryId,
                IsBookOfTheMonth = bookDto.IsBookOfTheMonth,
                BookPicture = "" // Initialize with an empty string or a default image path
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return new ServiceResponse { Status = ServiceResponse.ServiceStatus.Created, CreatedId = book.Id };
        }

        // Updates an existing book
        public async Task<ServiceResponse> UpdateBook(int id, BookDto bookDto)
        {
            var existingBook = await _context.Books.FindAsync(id);
            if (existingBook == null)
                return new ServiceResponse { Status = ServiceResponse.ServiceStatus.NotFound };

            existingBook.BookName = bookDto.BookName;
            existingBook.BookAuthor = bookDto.BookAuthor;
            existingBook.CategoryId = bookDto.CategoryId;
            existingBook.IsBookOfTheMonth = bookDto.IsBookOfTheMonth;

            _context.Books.Update(existingBook);
            await _context.SaveChangesAsync();

            return new ServiceResponse { Status = ServiceResponse.ServiceStatus.Success };
        }

        // Deletes a book
        public async Task<ServiceResponse> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return new ServiceResponse { Status = ServiceResponse.ServiceStatus.NotFound };

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return new ServiceResponse { Status = ServiceResponse.ServiceStatus.Success };
        }

        // Uploads an image for a book
        public async Task<ServiceResponse> UploadBookImage(int id, IFormFile bookImage)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return new ServiceResponse { Status = ServiceResponse.ServiceStatus.NotFound };

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (bookImage.Length > 0)
            {
                var filePath = Path.Combine(uploads, $"{book.Id}_{bookImage.FileName}");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await bookImage.CopyToAsync(stream);
                }

                book.BookPicture = $"/images/{book.Id}_{bookImage.FileName}";
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
            }

            return new ServiceResponse { Status = ServiceResponse.ServiceStatus.Success };
        }

        // Lists all reviews for a specific book
        public async Task<IEnumerable<ReviewDto>> ListReviewsForBook(int bookId)
        {
            return await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.Reviewers)
                .Select(r => new ReviewDto
                {
                    ReviewId = r.ReviewId,
                    ReviewText = r.ReviewText,
                    ReviewDate = r.ReviewDate,
                    BookId = r.BookId,
                    ReviewersId = r.ReviewersId,
                    ReviewersName = r.Reviewers.ReviewersName
                })
                .ToListAsync();
        }

        // Approves a review
        public async Task<ServiceResponse> ApproveReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return new ServiceResponse { Status = ServiceResponse.ServiceStatus.NotFound };
            }

            // Logic for approving the review can be added here

            return new ServiceResponse { Status = ServiceResponse.ServiceStatus.Success };
        }

        // Deletes a review
        public async Task<ServiceResponse> DeleteReview(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                return new ServiceResponse { Status = ServiceResponse.ServiceStatus.NotFound };
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return new ServiceResponse { Status = ServiceResponse.ServiceStatus.Success };
        }
    }
}
