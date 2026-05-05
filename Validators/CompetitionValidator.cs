using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class CompetitionValidator : AbstractValidator<Competitions>
    {
        public CompetitionValidator()
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("عنوان المسابقة مطلوب")
                .MaximumLength(50).WithMessage("العنوان طويل جداً");

            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("وصف المسابقة مطلوب");

            RuleFor(c => c.StartDate)
                .NotEmpty().WithMessage("تاريخ البدء مطلوب");

            RuleFor(c => c.EndDate)
                .NotEmpty().WithMessage("تاريخ الانتهاء مطلوب")
                .GreaterThan(c => c.StartDate).WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء");

            RuleFor(c => c.SubmissionDeadline)
                .LessThanOrEqualTo(c => c.EndDate).WithMessage("موعد التسليم النهائي لا يمكن أن يتجاوز تاريخ انتهاء المسابقة")
                .GreaterThan(c => c.StartDate).WithMessage("موعد التسليم يجب أن يكون بعد بداية المسابقة");

            RuleFor(c => c.Title)
    .NotEmpty().WithMessage("عنوان المسابقة مطلوب")
    .Must(t => !string.IsNullOrWhiteSpace(t))
    .WithMessage("العنوان لا يمكن أن يكون مسافات فقط")
    .MaximumLength(50);
        }
    }
}