using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Competitions
    {
        [Key]
        public int? CompetitionId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SubmissionDeadline { get; set; }

        public bool IsActive { get; set; } = true;

        public int? DepartmentId { get; set; }

        public string? ImageUrl { get; set; }
        public string? Category { get; set; }

        public string? PrizeDescription { get; set; }
        public string? JudgingCriteria { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
 
        public CompetitionDetails? CompetitionDetail { get; set; } = null!;
        public ICollection<Projects> Projects { get; set; } = new List<Projects>();
        public ICollection<CompetitionSubmissions> CompetitionSubmissions { get; set; } = new List<CompetitionSubmissions>();
    }
}