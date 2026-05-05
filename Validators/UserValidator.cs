using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class UserValidator : AbstractValidator<Users>
    {
        public UserValidator()
        {

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("البريد الإلكتروني مطلوب.")
                .Must(e => !string.IsNullOrWhiteSpace(e))
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");


            RuleFor(u => u.PasswordHash)
                .NotEmpty().WithMessage("كلمة المرور مطلوبة.")
                .MinimumLength(8).WithMessage("يجب أن تكون كلمة المرور 8 أحرف على الأقل.")
                .Matches("[A-Z]").WithMessage("يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل (A-Z).")
                .Matches("[a-z]").WithMessage("يجب أن تحتوي كلمة المرور على حرف صغير واحد على الأقل (a-z).")
                .Matches("[0-9]").WithMessage("يجب أن تحتوي كلمة المرور على رقم واحد على الأقل (0-9).")
                .Matches("[^a-zA-Z0-9]").WithMessage("يجب أن تحتوي كلمة المرور على رمز خاص واحد على الأقل (@، #، $، إلخ).");

  
        }
    }
}