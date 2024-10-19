using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Needed for IFormFile
using System.Collections.Generic;

namespace ReviewClubMvcpart.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string BookName { get; set; } = "";

        [Required, MaxLength(50)]
        public string BookAuthor { get; set; } = "";

        [Required]
        public int CategoryId { get; set; }

        public bool HasBookPic { get; set; }

        public string CategoryName { get; set; } = "";

        [Required]
        public bool IsBookOfTheMonth { get; set; }

        public string? BookPicture { get; set; }

        public IFormFile? BookImage { get; set; }

        // Get review count
        public int ReviewCount { get; set; }

        // List of reviews
        public List<BookReviewDto> Reviews { get; set; } = new List<BookReviewDto>();
    }

    public class CreateBookDto
    {
        [Required, MaxLength(50)]
        public string BookName { get; set; } = "";

        [Required, MaxLength(50)]
        public string BookAuthor { get; set; } = "";

        [Required]
        public int CategoryId { get; set; }

        // This property is for validation against the category name
        [Required, MaxLength(50)]
        public string CategoryName { get; set; } = "";

        [Required]
        public bool IsBookOfTheMonth { get; set; }

        public string? BookPicture { get; set; }

        public IFormFile? BookImage { get; set; }
    }

    public class UpdateBookDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string BookName { get; set; } = "";

        [Required, MaxLength(50)]
        public string BookAuthor { get; set; } = "";

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool IsBookOfTheMonth { get; set; }

        public string? BookPicture { get; set; }

        public IFormFile? BookImage { get; set; }
    }

    public class DeleteBookDto
    {
        [Required]
        public int Id { get; set; }
    }

    public class BookReviewDto
    {
        public int ReviewId { get; set; }
        public string ReviewText { get; set; } = "";
        public DateTime ReviewDate { get; set; }
        public string ReviewerName { get; set; } = "";
    }
}
