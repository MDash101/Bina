using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class RatingValidator : AbstractValidator<Rating>
    {
        public RatingValidator()
        {
            RuleFor(r => r.RatingValue)
                .InclusiveBetween(1, 5).WithMessage("التقييم يجب أن يكون بين 1 و 5 نجوم فقط");

            RuleFor(r => r.RatingCategory)
                .NotEmpty().WithMessage("يجب تحديد فئة التقييم (مثلاً: تعاون، مهارة تقنية)");

            RuleFor(r => r.RatingCategory)
    .NotEmpty().WithMessage("يجب تحديد فئة التقييم")
    .Must(c => !string.IsNullOrWhiteSpace(c))
    .WithMessage("فئة التقييم لا يمكن أن تكون مسافات فقط");

        }
    }
}