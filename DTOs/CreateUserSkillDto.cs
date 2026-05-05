using WebApplication_School.Enums;

namespace WebApplication_School.DTOs
{
    public class CreateUserSkillDto
    {
        public int UserId { get; set; }
        public int SkillId { get; set; }
        public ProficiencyLevel Level { get; set; }
        public string? ExperienceDescription { get; set; }
        public string? Notes { get; set; }
    }
}