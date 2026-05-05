using FluentValidation;
using WebApplication_School.Models;
using WebApplication_School.Services;

namespace WebApplication_School.Validators
{
    public class PrivateMessageValidator : AbstractValidator<PrivateMessages>
    {
        private readonly IUserService userService;

        public PrivateMessageValidator(IUserService userService)
        {
            this.userService = userService;

          
            RuleFor(m => m.Content)
                .NotEmpty().WithMessage("محتوى الرسالة لا يمكن أن يكون فارغاً")
                .Must(c => !string.IsNullOrWhiteSpace(c))
                .WithMessage("محتوى الرسالة لا يمكن أن يكون مسافات فقط")
                .MaximumLength(200).WithMessage("الرسالة طويلة جداً (الحد الأقصى 200 حرف)");

       
            RuleFor(m => m.SenderId)
                .GreaterThan(0)
                .Must(id => userService.Exists(id))
                .WithMessage("المرسل غير موجود");

            RuleFor(m => m.ReceiverId)
                .GreaterThan(0)
                .Must(id => userService.Exists(id))
                .WithMessage("المستلم غير موجود")
                .NotEqual(m => m.SenderId)
                .WithMessage("لا يمكنك إرسال رسالة لنفسك");
        }
    }
}