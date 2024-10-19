using ReviewClubMvcpart.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewClubMvcpart.Models
{
    public class Reviewer
    {
        [Key]
        public int ReviewersId { get; set; }

        [Required]
        public string ReviewerName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string ReviewersEmail { get; set; } = "";

        // A reviewer can write many reviews
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
