using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class UserCompetitionPreferenceValidator : AbstractValidator<UserCompetitionPreferences>
    {
        public UserCompetitionPreferenceValidator()
        {
            RuleFor(p => p.PreferenceName)
                .NotEmpty().WithMessage("اسم التفضيل مطلوب (مثل: برمجة، تصميم)")
                .MaximumLength(100).WithMessage("الاسم طويل جداً");

            RuleFor(p => p.CompetitionType)
      .Must(t => !string.IsNullOrWhiteSpace(t))
      .WithMessage("نوع المسابقة مطلوب")
      .MaximumLength(50);

            RuleFor(p => p.UserId)
                .GreaterThan(0).WithMessage("يجب تحديد مستخدم صحيح");

            RuleFor(p => p.CompetitionId)
                .GreaterThan(0).WithMessage("يجب تحديد مسابقة صحيحة");

            RuleFor(p => p.PreferenceName)
    .NotEmpty().WithMessage("اسم التفضيل مطلوب")
    .Must(n => !string.IsNullOrWhiteSpace(n))
    .WithMessage("الاسم لا يمكن أن يكون مسافات فقط")
    .MaximumLength(100);
        }
    }
}