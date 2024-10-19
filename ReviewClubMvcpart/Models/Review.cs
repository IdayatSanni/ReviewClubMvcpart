using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReviewClubMvcpart.Models
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
        public int? BookId { get; set; } // Make BookId nullable
        public virtual Book? Book { get; set; } // Make Book nullable

        [ForeignKey("Reviewer")]
        public int? ReviewersId { get; set; } // Make ReviewersId nullable
        public virtual Reviewer? Reviewer { get; set; } // Make Reviewer nullable

        public bool IsApproved { get; set; } = false;
    }
}
