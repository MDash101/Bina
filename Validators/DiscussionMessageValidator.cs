using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
  
    public class DiscussionMessagesValidator : AbstractValidator<DiscussionMessages>
    {
        public DiscussionMessagesValidator()
        {
            RuleFor(m => m.Content)
                .NotEmpty().WithMessage("لا يمكن إرسال رسالة فارغة")
                .MaximumLength(300).WithMessage("الرسالة طويلة جداً (الحد الأقصى 300 حرف)");

            RuleFor(m => m.DiscussionId)
                .GreaterThan(0).WithMessage("يجب أن تنتمي الرسالة لمناقشة معينة");

            RuleFor(m => m.Content)
    .NotEmpty().WithMessage("لا يمكن إرسال رسالة فارغة")
    .Must(c => !string.IsNullOrWhiteSpace(c))
    .WithMessage("الرسالة لا يمكن أن تكون مسافات فقط")
    .MaximumLength(300);
        }
    }
}