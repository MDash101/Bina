using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class CompetitionSubmissions
    {
        [Key]
        public int SubmissionId { get; set; }

        public int CompetitionId { get; set; }
        public int ProjectId { get; set; }

        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string Status { get; set; } = "Pending";

        [Range(0, 100)]
        public int? Score { get; set; }

        public string? JudgeComments { get; set; }

        [StringLength(100)]
        public string? AwardType { get; set; }

        [ForeignKey("CompetitionId")]
        public virtual Competitions Competition { get; set; } = null!;

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; } = null!;
    }
}