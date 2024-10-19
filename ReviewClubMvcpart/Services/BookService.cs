using Microsoft.EntityFrameworkCore;
using ReviewClubMvcpart.Data;
using ReviewClubMvcpart.Dtos;
using ReviewClubMvcpart.Interfaces;
using ReviewClubMvcpart.Models;
using Microsoft.Extensions.Logging;

namespace ReviewClubMvcpart.Services
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookService> _logger;

        public BookService(ApplicationDbContext context, ILogger<BookService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<BookDto>> ListBooks()
        {
            var books = await _context.Books
                .Include(b => b.Reviews)
                .Include(b => b.Category)
                .ToListAsync();

            return books.Select(book => new BookDto
            {
                Id = book.Id,
                BookName = book.BookName,
                BookAuthor = book.BookAuthor,
                CategoryId = book.CategoryId,
                BookPicture = book.BookPicture,
                HasBookPic = book.HasPic,
                CategoryName = book.Category?.BookCategory ?? "Unknown", // Handle null category
                IsBookOfTheMonth = book.IsBookOfTheMonth,
                ReviewCount = book.Reviews.Count,
                Reviews = book.Reviews.Select(review => new BookReviewDto
                {
                    ReviewId = review.ReviewId,
                    ReviewText = review.ReviewText,
                    ReviewDate = review.ReviewDate,
                    ReviewerName = review.Reviewer != null ? review.Reviewer.ReviewerName : "Unknown" // Handle null reviewer
                }).ToList()
            }).ToList();
        }

        public async Task<BookDto?> FindBook(int id)
        {
            var book = await _context.Books
                .Include(b => b.Reviews)
                .ThenInclude(r => r.Reviewer)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            return new BookDto
            {
                Id = book.Id,
                BookName = book.BookName,
                BookAuthor = book.BookAuthor,
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.BookCategory ?? "Unknown", // Handle null category
                IsBookOfTheMonth = book.IsBookOfTheMonth,
                ReviewCount = book.Reviews.Count,
                Reviews = book.Reviews.Select(review => new BookReviewDto
                {
                    ReviewId = review.ReviewId,
                    ReviewText = review.ReviewText,
                    ReviewDate = review.ReviewDate,
                    ReviewerName = review.Reviewer?.ReviewerName ?? "Unknown" // Handle null reviewer
                }).ToList()
            };
        }

        public async Task<ServiceResponse> AddBook(CreateBookDto createBookDto)
        {
            var response = new ServiceResponse();

            // Check if the category exists and if the name matches
            var category = await _context.Categories.FindAsync(createBookDto.CategoryId);
            if (category == null || category.BookCategory != createBookDto.CategoryName)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Invalid category ID or category name does not match.");
                return response;
            }

            // Proceed with book creation
            var book = new Book
            {
                BookName = createBookDto.BookName,
                BookAuthor = createBookDto.BookAuthor,
                CategoryId = createBookDto.CategoryId,
                IsBookOfTheMonth = createBookDto.IsBookOfTheMonth,
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            response.Status = ServiceResponse.ServiceStatus.Created;
            response.CreatedId = book.Id;
            return response;
        }

        public async Task<ServiceResponse> UpdateBook(UpdateBookDto updateBookDto)
        {
            var response = new ServiceResponse();

            var book = await _context.Books.FindAsync(updateBookDto.Id);
            if (book == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Book not found");
                return response;
            }

            // Validation checks
            if (string.IsNullOrWhiteSpace(updateBookDto.BookName))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Book name cannot be empty.");
                return response;
            }

            if (string.IsNullOrWhiteSpace(updateBookDto.BookAuthor))
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Book author cannot be empty.");
                return response;
            }

            if (updateBookDto.CategoryId <= 0)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Invalid category ID.");
                return response;
            }

            // Update the book properties
            book.BookName = updateBookDto.BookName;
            book.BookAuthor = updateBookDto.BookAuthor;
            book.CategoryId = updateBookDto.CategoryId;
            book.IsBookOfTheMonth = updateBookDto.IsBookOfTheMonth;

            try
            {
                // Handle image upload if provided
                if (updateBookDto.BookImage != null)
                {
                    var uploadResponse = await UploadBookImage(book.Id, updateBookDto.BookImage);
                    if (uploadResponse.Status != ServiceResponse.ServiceStatus.Updated)
                    {
                        return uploadResponse; // Return if there was an error
                    }
                }

                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Updated;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("An error occurred updating the book.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteBook(int id)
        {
            var response = new ServiceResponse();

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Book not found");
                return response;
            }

            try
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                response.Status = ServiceResponse.ServiceStatus.Deleted;
            }
            catch (DbUpdateException ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error occurred while deleting the book.");
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        public async Task<ServiceResponse> UploadBookImage(int id, IFormFile bookImage)
        {
            var response = new ServiceResponse();

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"Book {id} not found");
                return response;
            }

            if (bookImage?.Length > 0)
            {
                // Validate file type
                var validExtensions = new List<string> { ".jpeg", ".jpg", ".png", ".gif" };
                var bookImageExtension = Path.GetExtension(bookImage.FileName).ToLowerInvariant();
                if (!validExtensions.Contains(bookImageExtension))
                {
                    response.Messages.Add($"{bookImageExtension} is not a valid file extension");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    return response;
                }

                // Create a unique filename
                var fileName = $"{id}{bookImageExtension}";
                var filePath = Path.Combine("wwwroot/images/books/", fileName);

                // Remove old picture if exists
                if (!string.IsNullOrEmpty(book.BookPicture))
                {
                    var oldFilePath = Path.Combine("wwwroot/images/books/", book.BookPicture);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                // Save the new image
                using (var targetStream = File.Create(filePath))
                {
                    await bookImage.CopyToAsync(targetStream);
                }

                book.BookPicture = fileName;
                book.HasPic = true; // Set HasBookPic to true

                try
                {
                    await _context.SaveChangesAsync();
                    response.Status = ServiceResponse.ServiceStatus.Updated;
                }
                catch (DbUpdateException ex)
                {
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    response.Messages.Add("Error occurred while saving the book image.");
                    response.Messages.Add(ex.Message);
                }
            }
            else
            {
                response.Messages.Add("No file content");
                response.Status = ServiceResponse.ServiceStatus.Error;
            }

            return response;
        }
    }
}
