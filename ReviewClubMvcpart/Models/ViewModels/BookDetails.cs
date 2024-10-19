using ReviewClubMvcpart.Dtos;
using System.Collections.Generic;

namespace ReviewClubMvcpart.Models.ViewModels
{
    public class BookDetails
    {
        public BookDto Book { get; set; } = new BookDto();
        public IEnumerable<CategoryDto> AllCategories { get; set; } = new List<CategoryDto>();
        public IEnumerable<ReviewDto> BookReviews { get; set; } = new List<ReviewDto>();
    }
}
