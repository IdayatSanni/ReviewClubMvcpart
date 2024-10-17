using System.ComponentModel.DataAnnotations;


namespace ReviewClubCms.Dtos
{
    public class BookDto
    {
        public int? Id { get; set; }

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

    public class createBook
    {
        public int? Id { get; set; }

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

    public class updateBook { 
        public int? Id { get; set; }
        [Required, MaxLength(50)]
        public string BookName { get; set; } = "";

        [Required, MaxLength(50)]
        public string BookAuthor { get; set; } = "";

        [Required]
        public int? CategoryId { get; set; }
    }

    public class BookWithReviewsDto
    {
        public int? Id { get; set; }

        public string BookName { get; set; } = "";

        public string BookAuthor { get; set; } = "";

        public int CategoryId { get; set; }

        public bool IsBookOfTheMonth { get; set; }

        public string? BookPicture { get; set; }

        public int ReviewCount { get; set; } // Add this to display review count
    }

}