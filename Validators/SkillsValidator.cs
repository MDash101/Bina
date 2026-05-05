using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class SkillsValidator : AbstractValidator<Skills>
    {
        public SkillsValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("اسم المهارة مطلوب")
                .MaximumLength(30).WithMessage("اسم المهارة يجب أن يكون مختصراً (مثل: C#, React)");

            RuleFor(s => s.DifficultyLevel)
      .IsInEnum()
      .WithMessage("مستوى المهارة غير صحيح");
        }
    }
}