using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Rating
    {
        [Key]
        public int RatingId { get; set; }

        public int RatedUserId { get; set; }
        public int RaterUserId { get; set; }
        public int ProjectId { get; set; }

        [Range(1, 5)]
        public int RatingValue { get; set; }

        public string? Comment { get; set; }

        [Required, StringLength(100)]
        public string RatingCategory { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProjectId")]
        public virtual Projects? Project { get; set; }

        [ForeignKey("RatedUserId")]
        public virtual Users? RatedUser { get; set; }

        [ForeignKey("RaterUserId")]
        public virtual Users RaterUser { get; set; } = null!;
    }
}