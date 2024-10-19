using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewClubMvcpart.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string BookCategory { get; set; } = "";
        public virtual ICollection<Book> Books { get; set; } = new List<Book>(); // Navigation property
    }
}
