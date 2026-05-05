using WebApplication_School.Enums;

namespace WebApplication_School.DTOs
{
    public class UpdateUserSkillDto
    {
        public ProficiencyLevel Level { get; set; }

        public bool IsVerified { get; set; }

        public int? VerifiedBy { get; set; }

        public DateTime? VerificationDate { get; set; }

        public string? ExperienceDescription { get; set; }

        public string? Notes { get; set; }
    }
}