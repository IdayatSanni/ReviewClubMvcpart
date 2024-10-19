using ReviewClubMvcpart.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Book
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string BookName { get; set; } = "";

    [Required]
    [MaxLength(50)]
    public string BookAuthor { get; set; } = "";

    [Required] // Indicates that every book must belong to a category
    public int CategoryId { get; set; }

    public string? BookPicture { get; set; }

    public bool HasPic { get; set; } = false;

    [Required]
    public bool IsBookOfTheMonth { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Review>? Reviews { get; set; } // Make Reviews nullable
}
