using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class UserSkillValidator : AbstractValidator<UserSkills>
    {
        public UserSkillValidator()
        {
            RuleFor(s => s.Level)
     .IsInEnum()
     .WithMessage("مستوى الإتقان غير صحيح");

            RuleFor(s => s.SkillId)
                .GreaterThan(0).WithMessage("يجب تحديد المهارة بشكل صحيح");

            RuleFor(s => s.UserId)
                .GreaterThan(0).WithMessage("يجب ربط المهارة بمستخدم");

            RuleFor(s => s.Notes)
                .MaximumLength(200).WithMessage("الملاحظات لا يجب أن تتجاوز 200 حرف");
        }
    }
}