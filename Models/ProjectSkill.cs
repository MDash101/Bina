using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class ProjectSkills
    {

        public enum SkillLevel
        {
            Beginner = 1,
            Intermediate = 2,
            Advanced = 3
        }

        public int ProjectSkillId { get; set; }

        public int ProjectId { get; set; }

        public int SkillId { get; set; }

        public int SkillLevelRequired { get; set; }

        public bool IsFilled { get; set; } = false;

        public int? FilledByUserId { get; set; }

        public string? Notes { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; } = null!;

        [ForeignKey("SkillId")]
        public virtual Skills Skill { get; set; } = null!;

        [ForeignKey("FilledByUserId")]
        public virtual Users? FilledByUser { get; set; }
    }
}