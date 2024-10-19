using System;
using System.ComponentModel.DataAnnotations;

namespace ReviewClubMvcpart.Dtos
{
    public class ReviewDto
    {
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ReviewText { get; set; } = "";

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public int? BookId { get; set; } // Make BookId nullable
        public int? ReviewersId { get; set; } // Make ReviewersId nullable

        public string? BookName { get; set; } // Make BookName nullable
        public string? ReviewerName { get; set; } // Make ReviewerName nullable
    }

    public class CreateReviewDto
    {
        [Required]
        [MaxLength(1000)]
        public string ReviewText { get; set; } = "";

        [Required]
        public int? ReviewersId { get; set; } // Make ReviewersId nullable

        [Required]
        public int? BookId { get; set; } // Make BookId nullable
    }

    public class UpdateReviewDto
    {
        [Required]
        public int ReviewId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string ReviewText { get; set; } = "";
    }

    public class DeleteReviewDto
    {
        [Required]
        public int ReviewId { get; set; }
    }
}
