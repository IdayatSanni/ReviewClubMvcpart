using System.ComponentModel.DataAnnotations;

namespace ReviewClubMvcpart.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required, MaxLength(25)]
        public string BookCategory { get; set; } = "";

        // Navigation property for books
        public virtual ICollection<BookDto> Books { get; set; } = new List<BookDto>();

        public string? Title { get; set; }
        public int? BookCount { get; set; }
    }

    public class CreateCategoryDto
    {

        [Required, MaxLength(25)]
        public string BookCategory { get; set; } = "";
    }

    public class UpdateCategory
    {
        public int Id { get; set; }
        [Required, MaxLength(25)]
        public string BookCategory { get; set; } = "";
    }

    public class DeleteCategory
    {
        public int Id { get; set; }
    }
}
