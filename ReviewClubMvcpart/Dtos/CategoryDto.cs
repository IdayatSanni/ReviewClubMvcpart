using System.ComponentModel.DataAnnotations;

namespace ReviewClubCms.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        [Required, MaxLength(25)]
        public string BookCategory { get; set; } = "";
    }

    public class updateCategory
    {
        public int Id { get; set; }
        [Required, MaxLength(25)]
        public string BookCategory { get; set; } = "";
    }

    public class deleteCategory
    {
        public int Id { get; set; }
    }
}
