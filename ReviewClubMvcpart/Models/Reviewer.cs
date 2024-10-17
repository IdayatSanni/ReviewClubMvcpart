using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewClubCms.Models
{
    public class Reviewer
    {
        [Key]
        public int ReviewersId { get; set; }

        [Required]
        public string ReviewersName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string ReviewersEmail { get; set; } = "";

        // Navigation property for the reviews written by this reviewer
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
