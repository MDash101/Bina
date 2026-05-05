using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class ProjectValidator : AbstractValidator<Projects>
    {
        public ProjectValidator()
        {
            RuleFor(p => p.Title)
           .NotEmpty().WithMessage("عنوان المشروع مطلوب")
           .MaximumLength(20).WithMessage("العنوان طويل جداً");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("وصف المشروع لا يمكن أن يكون فارغاً")
                .MinimumLength(20).WithMessage("الوصف يجب أن يكون 20 حرفاً على الأقل لشرح فكرة المشروع");

            RuleFor(p => p.OwnerId)
                .GreaterThan(0).WithMessage("يجب تحديد صاحب المشروع");
        }
    }
}