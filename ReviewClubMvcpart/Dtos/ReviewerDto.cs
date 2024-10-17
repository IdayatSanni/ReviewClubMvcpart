using System.ComponentModel.DataAnnotations;

namespace ReviewClubCms.Dtos
{
    public class ReviewerDto
    {
        public int ReviewersId { get; set; }

        [Required]
        public string ReviewersName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string ReviewersEmail { get; set; } = "";

        public int ReviewedBookCount { get; set; }
    }

    public class createReviewerDto
    {
        public int ReviewersId { get; set; }

        [Required]
        public string ReviewersName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string ReviewersEmail { get; set; } = "";

        public int ReviewedBookCount { get; set; }
    }

    public class updateReviewerDto
    {
        public int ReviewersId { get; set; }

        [Required]
        public string ReviewersName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string ReviewersEmail { get; set; } = "";

        public int ReviewedBookCount { get; set; }
    }

    public class deleteReviewerDto
    {
        public int ReviewersId { get; set; }

        [Required]
        public string ReviewersName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string ReviewersEmail { get; set; } = "";

        public int ReviewedBookCount { get; set; }
    }
}
