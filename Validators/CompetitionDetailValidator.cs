using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    
    public class CompetitionDetailValidator : AbstractValidator<CompetitionDetails>
    {
        public CompetitionDetailValidator()
        {
            RuleFor(cd => cd.Title)
     .NotEmpty().WithMessage("عنوان تفاصيل المسابقة مطلوب")
     .Must(t => !string.IsNullOrWhiteSpace(t))
     .WithMessage("العنوان لا يمكن أن يكون فارغ أو مسافات فقط")
     .MaximumLength(150);

            RuleFor(cd => cd.Description)
                .NotEmpty().WithMessage("وصف التفاصيل مطلوب")
                .MinimumLength(10);

            RuleFor(cd => cd.StartDate)
                .GreaterThan(DateTime.MinValue);

            RuleFor(cd => cd.EndDate)
                .GreaterThan(cd => cd.StartDate)
                .WithMessage("تاريخ الانتهاء يجب أن يكون بعد تاريخ البدء");

            RuleFor(cd => cd.SubmissionDeadline)
      .LessThanOrEqualTo(cd => cd.EndDate)
      .WithMessage("موعد التسليم لا يمكن أن يتجاوز تاريخ انتهاء المسابقة");

            RuleFor(cd => cd.MinParticipants)
                .GreaterThan(0);

            RuleFor(cd => cd.MaxParticipants)
                .GreaterThanOrEqualTo(cd => cd.MinParticipants);

            RuleFor(cd => cd.RulesUrl)
                .NotEmpty()
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _));

           
            RuleFor(cd => cd.PrizeDetails)
                .MaximumLength(100).WithMessage("تفاصيل الجوائز طويلة جداً");
        }
    }
}