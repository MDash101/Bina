using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class ProjectDiscussionValidator : AbstractValidator<ProjectDiscussions>
    {
        public ProjectDiscussionValidator()
        {
         
            RuleFor(pd => pd.Title)
                .NotEmpty().WithMessage("يجب إدخال عنوان للمناقشة.")
                .MaximumLength(100).WithMessage("العنوان طويل جداً (الحد الأقصى 100 حرف).");


            RuleFor(pd => pd.ProjectId)
      .GreaterThan(0)
      .WithMessage("يجب اختيار مشروع صحيح");

            RuleFor(pd => pd.CreatedById)
                .GreaterThan(0)
                .WithMessage("يجب تحديد المستخدم المنشئ");


            RuleFor(pd => pd.CreatedDate)
                .Must(date => date <= DateTime.UtcNow).WithMessage("تاريخ الإنشاء لا يمكن أن يكون في المستقبل.");

            RuleFor(pd => pd.Title)
    .NotEmpty().WithMessage("يجب إدخال عنوان للمناقشة.")
    .Must(t => !string.IsNullOrWhiteSpace(t))
    .WithMessage("العنوان لا يمكن أن يكون مسافات فقط")
    .MaximumLength(20).WithMessage("العنوان طويل جداً (الحد الأقصى 20 حرف).");

        }
    }
}