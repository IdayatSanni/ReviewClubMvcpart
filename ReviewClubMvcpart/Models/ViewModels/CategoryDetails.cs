using ReviewClubMvcpart.Dtos;

namespace ReviewClubMvcpart.Models.ViewModels
{
    public class CategoryDetails
    {
        public required CategoryDto Category { get; set; }
        public IEnumerable<BookDto>? BooksInCategory { get; set; }
    }
}
