using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReviewClubMvcpart.Dtos
{
    // Represents a reviewer with basic information
    public class ReviewerDto
    {
        
        public int ReviewerId { get; set; }

        [Required, MaxLength(25)]
        public string ReviewerName { get; set; } = "";

        [Required, MaxLength(25)]
        [EmailAddress]
        public string ReviewerEmail { get; set; } = "";

        public int ReviewedBookCount { get; set; } // Can be derived from reviews

        // List of reviews associated with the reviewer
        public List<ReviewInfoDto> Reviews { get; set; } = new List<ReviewInfoDto>();
    }

    // Represents details of each review
    public class ReviewInfoDto
    {
        public int ReviewId { get; set; } // ID for editing or deleting the review
        public string BookName { get; set; } = "";
        public string ReviewText { get; set; } = "";
        public DateTime ReviewDate { get; set; }
    }

    // Used for creating a new reviewer
    public class CreateReviewerDto
    {

        [Required, MaxLength(25)]
        public string ReviewerName { get; set; } = "";

        [Required, MaxLength(25)]
        public string ReviewerEmail { get; set; } = "";
    }

    // Used for updating an existing reviewer
    public class UpdateReviewerDto
    {
        public int ReviewerId { get; set; }

        [Required]
        public string ReviewerName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string ReviewerEmail { get; set; } = "";

        public int ReviewedBookCount { get; set; } // Optional; can be derived from reviews
    }

    // Combines reviewer information with their associated reviews
    public class ReviewerWithReviewsDto
    {
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; } = "";
        public string ReviewerEmail { get; set; } = "";
        public List<ReviewInfoDto> Reviews { get; set; } = new List<ReviewInfoDto>();
    }

    // Used when deleting a reviewer
    public class DeleteReviewerDto
    {
        public int ReviewerId { get; set; } // Only need the ID for deletion
    }
}
