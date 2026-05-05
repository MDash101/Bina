using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class CompetitionDetails
    {
        [Key]
        public int CompetitionDetailId { get; set; }

        public int CompetitionId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public string? EvaluationCriteria { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SubmissionDeadline { get; set; }
      
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }

        [Required, StringLength(50)]
        public string CompetitionType { get; set; } = null!;

        public string? PrizeDetails { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(255)]
        public string? RulesUrl { get; set; }

        [ForeignKey("CompetitionId")]
        public virtual Competitions Competition { get; set; } = null!;
    }
}