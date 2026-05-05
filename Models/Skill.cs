using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Skills
    {
      

        public enum SkillLevel
        {
            Beginner = 1,
            Intermediate = 2,
            Advanced = 3
        }
        public int SkillId { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; } = null!;

        [Required, StringLength(255)]
        public string Category { get; set; } = null!;

        public int? DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        public string Description { get; set; } = string.Empty;

        public int DifficultyLevel { get; set; }

        public bool IsCoreSkills { get; set; } = false;
        public ICollection<UserSkills> UserSkills { get; set; } = new List<UserSkills>();
        public ICollection<ProjectSkills> ProjectSkills { get; set; } = new List<ProjectSkills>();
    }
}