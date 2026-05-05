using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication_School.Enums;
namespace WebApplication_School.Models
{
    public class UserSkills
    {
        [Key]
        public int UserSkillId { get; set; }

        public int UserId { get; set; }

        public int SkillId { get; set; }

        public ProficiencyLevel Level { get; set; }

        public bool IsVerified { get; set; } = false;

        public int? VerifiedBy { get; set; }

        public DateTime? VerificationDate { get; set; }

        public string? ExperienceDescription { get; set; }

        public string? Notes { get; set; }

        public int EndorseCount { get; set; } = 0;

        public int? LastEndorsedBy { get; set; }

        public DateTime? LastEndorsedDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("SkillId")]
        public virtual Skills Skill { get; set; } = null!;

        [ForeignKey("VerifiedBy")]
        public virtual Users? VerifiedByUser { get; set; }

        [ForeignKey("LastEndorsedBy")]
        public virtual Users? LastEndorsedByUser { get; set; }
    }
}