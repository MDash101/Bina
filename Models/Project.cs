using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Projects
    {
        [Key]
        public int ProjectsId { get; set; }

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        public int OwnerId { get; set; }
        public int? DepartmentId { get; set; }
        public int? CompetitionId { get; set; }

        public string Status { get; set; } = null!;
        public int Complexity { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpectedEndDate { get; set; }

        public bool IsCompetitionEntry { get; set; }
        public bool IsPublic { get; set; }

        public int ViewCount { get; set; }

        public decimal MilestoneCompletionPercentage { get; set; }
        public bool MilestoneIsCompleted { get; set; }

        [ForeignKey("OwnerId")]
        public Users Owner { get; set; } = null!;

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        [ForeignKey("CompetitionId")]
        public Competitions? Competition { get; set; }

        public ICollection<ProjectMembers> ProjectMembers { get; set; } = new List<ProjectMembers>();
        public ICollection<ProjectMedia> ProjectMedia { get; set; } = new List<ProjectMedia>();
        public ICollection<ProjectDiscussions> ProjectDiscussions { get; set; } = new List<ProjectDiscussions>();
        public ICollection<Comments> Comments { get; set; } = new List<Comments>();

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<CompetitionSubmissions> CompetitionSubmissions { get; set; } = new List<CompetitionSubmissions>();
        public ICollection<ProjectSkills> ProjectSkills { get; set; } = new List<ProjectSkills>();
        public ICollection<ProjectTasks> ProjectTasks { get; set; } = new List<ProjectTasks>();
    }
}