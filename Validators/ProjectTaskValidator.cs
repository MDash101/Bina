using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class ProjectTaskValidator : AbstractValidator<ProjectTasks>
    {
        public ProjectTaskValidator()
        {
            RuleFor(t => t.Title)
      .NotEmpty().WithMessage("عنوان المهمة مطلوب")
      .Must(ti => !string.IsNullOrWhiteSpace(ti))
      .WithMessage("العنوان لا يمكن أن يكون مسافات فقط")
      .MaximumLength(100);

            RuleFor(t => t.Priority)
      .IsInEnum()
      .WithMessage("الأولوية غير صحيحة");

            RuleFor(t => t.ProjectId).GreaterThan(0);
            RuleFor(t => t.CreatedBy).GreaterThan(0);
        }
    }
}