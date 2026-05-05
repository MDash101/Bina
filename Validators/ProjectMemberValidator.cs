using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class ProjectMemberValidator : AbstractValidator<ProjectMembers>
    {
        public ProjectMemberValidator()
        {
            RuleFor(m => m.Role)
                .NotEmpty().WithMessage("يجب تحديد دور العضو في المشروع (مثلاً: مطور، مصمم)");

            RuleFor(m => m.ProjectId)
                .GreaterThan(0).WithMessage("يجب تحديد مشروع صحيح");

            RuleFor(m => m.UserId)
                .GreaterThan(0).WithMessage("يجب تحديد مستخدم صحيح");

            RuleFor(m => m.Status)
        .Must(s => string.IsNullOrEmpty(s) || new[] { "Active", "Pending", "Rejected" }
        .Contains(s))
        .WithMessage("الحالة غير صحيحة");

            RuleFor(m => m.Role)
    .NotEmpty().WithMessage("يجب تحديد دور العضو في المشروع")
    .Must(r => !string.IsNullOrWhiteSpace(r))
    .WithMessage("الدور لا يمكن أن يكون مسافات فقط")
    .MaximumLength(50);
        }
    }
}