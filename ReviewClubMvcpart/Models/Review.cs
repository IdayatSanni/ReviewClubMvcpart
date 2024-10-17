using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReviewClubCms.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ReviewText { get; set; } = "";

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("Book")]
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = new();

        [ForeignKey("Reviewers")]
        public int ReviewersId { get; set; }
        public virtual Reviewer Reviewers { get; set; } = new();

        public bool IsApproved { get; set; } = false; // New property for approval status
    }
}
