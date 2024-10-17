using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReviewClubCms.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string BookName { get; set; } = "";

        [MaxLength(50)]
        public string BookAuthor { get; set; } = "";

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string BookPicture { get; set; } = "";

        [Required]
        public bool IsBookOfTheMonth { get; set; }

        // Navigation property for category
        public virtual Category Category { get; set; } = new();

        // Navigation property for reviews
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

        
    }
}
