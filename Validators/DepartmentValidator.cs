using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class DepartmentValidator : AbstractValidator<Department>
    {
        public DepartmentValidator()
        {

            RuleFor(d => d.Name)
                .NotEmpty().WithMessage("اسم القسم لا يمكن أن يكون فارغاً.")
                .MinimumLength(10).WithMessage("اسم القسم قصير جداً.")
                .MaximumLength(100).WithMessage("اسم القسم لا يجب أن يتجاوز 100 حرف.");


            RuleFor(d => d.Description)
                .NotEmpty().WithMessage("يجب إضافة وصف بسيط للقسم.")
                .MaximumLength(200).WithMessage("الوصف طويل جداً (الحد الأقصى 200 حرف).");


            RuleFor(d => d.ContactEmail)
                .NotEmpty().WithMessage("البريد الإلكتروني للقسم مطلوب.")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة.");

            RuleFor(d => d.ContactEmail)
    .NotEmpty()
    .EmailAddress()
    .MaximumLength(100).WithMessage("البريد الإلكتروني طويل جداً");


            RuleFor(d => d.HeadId)
                .GreaterThan(0).WithMessage("معرف رئيس القسم يجب أن يكون رقماً صحيحاً.")
                .When(d => d.HeadId.HasValue);

            RuleFor(d => d.Name)
    .NotEmpty().WithMessage("اسم القسم لا يمكن أن يكون فارغاً.")
    .Must(n => !string.IsNullOrWhiteSpace(n))
    .WithMessage("اسم القسم لا يمكن أن يكون مسافات فقط")
    .MinimumLength(10)
    .MaximumLength(100);
        }
    }
}