using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class ProjectSkillValidator : AbstractValidator<ProjectSkills>
    {
        public ProjectSkillValidator()
        {
            RuleFor(ps => ps.SkillLevelRequired)
                .InclusiveBetween(1, 5).WithMessage("مستوى المهارة المطلوب يجب أن يكون بين 1 و 5");

            RuleFor(ps => ps.ProjectId).GreaterThan(0);
            RuleFor(ps => ps.SkillId).GreaterThan(0);

            RuleFor(ps => ps.Notes)
                .MaximumLength(200).WithMessage("الملاحظات يجب أن تكون مختصرة");

            RuleFor(ps => ps.ProjectId)
    .GreaterThan(0)
    .WithMessage("يجب اختيار مشروع صحيح");

            RuleFor(ps => ps.SkillId)
                .GreaterThan(0)
                .WithMessage("يجب اختيار مهارة صحيحة");

            RuleFor(ps => ps.Notes)
    .Must(n => string.IsNullOrEmpty(n) || !string.IsNullOrWhiteSpace(n))
    .WithMessage("الملاحظات لا يمكن أن تكون مسافات فقط")
    .MaximumLength(200);
        }
    }
}