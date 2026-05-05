using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class NotificationValidator : AbstractValidator<Notifications>
    {
        public NotificationValidator()
        {
            RuleFor(n => n.Title)
                .NotEmpty().WithMessage("عنوان الإشعار مطلوب");

            RuleFor(n => n.Title)
    .MaximumLength(50).WithMessage("عنوان الإشعار طويل جداً");

            RuleFor(n => n.Content)
                .NotEmpty().WithMessage("محتوى الإشعار مطلوب")
                .MaximumLength(60).WithMessage("محتوى الإشعار طويل جداً");

            RuleFor(n => n.UserId).GreaterThan(0);
            RuleFor(n => n.Type).NotEmpty().WithMessage("يجب تحديد نوع الإشعار");

            RuleFor(n => n.Title)
    .NotEmpty().WithMessage("عنوان الإشعار مطلوب")
    .Must(t => !string.IsNullOrWhiteSpace(t))
    .WithMessage("العنوان لا يمكن أن يكون مسافات فقط");

            RuleFor(n => n.Content)
    .NotEmpty().WithMessage("محتوى الإشعار مطلوب")
    .Must(c => !string.IsNullOrWhiteSpace(c))
    .WithMessage("المحتوى لا يمكن أن يكون مسافات فقط")
    .MaximumLength(30);

            RuleFor(n => n.UserId)
    .GreaterThan(0)
    .WithMessage("يجب تحديد مستخدم صحيح");

            RuleFor(n => n.Type)
    .NotEmpty()
    .Must(t => new[] {"Warning", "Success", "Error" }
    .Contains(t))
    .WithMessage("نوع الإشعار غير صحيح");
        }
    }
}