using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class CommentValidator : AbstractValidator<Comments>
    {
        public CommentValidator()
        {
            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("لا يمكن إضافة تعليق فارغ")
                .MaximumLength(1000).WithMessage("التعليق طويل جداً");

            RuleFor(c => c.ProjectId)
                .GreaterThan(0).WithMessage("يجب تحديد المشروع");

            RuleFor(c => c.UserId)
                .GreaterThan(0).WithMessage("يجب تحديد المستخدم");
        }
    }
}