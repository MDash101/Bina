using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class AwardValidator : AbstractValidator<Awards>
    {
        public AwardValidator()
        {
            RuleFor(a => a.Title)
      .NotEmpty()
      .Must(t => !string.IsNullOrWhiteSpace(t))
      .WithMessage("عنوان الجائزة مطلوب");

            RuleFor(a => a.Type)
       .NotEmpty()
       .Must(t => !string.IsNullOrWhiteSpace(t))
       .WithMessage("نوع الجائزة مطلوب");

            RuleFor(a => a.PointsAwarded)
                .GreaterThanOrEqualTo(0).WithMessage("النقاط الممنوحة يجب أن تكون صفراً أو أكثر");

            RuleFor(a => a.AwardedDate)
                .Must(date => date <= DateTime.UtcNow).WithMessage("تاريخ الجائزة لا يمكن أن يكون في المستقبل");
        }
    }
}