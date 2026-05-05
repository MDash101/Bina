using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class CompetitionSubmissionValidator : AbstractValidator<CompetitionSubmissions>
    {
        public CompetitionSubmissionValidator()
        {

            RuleFor(s => s.CompetitionId)
                .GreaterThan(0).WithMessage("يجب تحديد المسابقة التابع لها التسليم.");

            RuleFor(s => s.ProjectId)
                .GreaterThan(0).WithMessage("يجب تحديد المشروع الذي يتم تسليمه.");

            RuleFor(s => s.JudgeComments)
      .Must(c => string.IsNullOrEmpty(c) || !string.IsNullOrWhiteSpace(c))
      .WithMessage("تعليق غير صالح");

            RuleFor(s => s.Status)
                .NotEmpty().WithMessage("حالة التسليم مطلوبة (مثلاً: Pending, Approved, Rejected).")
                .MaximumLength(50).WithMessage("وصف الحالة طويل جداً.");

            RuleFor(s => s.Score)
    .NotNull().When(s => s.Status == "Approved")
    .WithMessage("يجب إدخال درجة عند قبول المشروع");

            RuleFor(s => s.Score)
                .InclusiveBetween(0, 100).WithMessage("الدرجة يجب أن تكون بين 0 و 100.")
                .When(s => s.Score.HasValue);

            RuleFor(s => s.JudgeComments)
                .MaximumLength(100).WithMessage("تعليقات الحكام لا يجب أن تتجاوز 100 حرف.");

            RuleFor(s => s.Status)
    .Must(status => new[] { "Pending", "Approved", "Rejected" }.Contains(status))
    .WithMessage("حالة التسليم غير صحيحة");

            RuleFor(s => s.SubmittedDate)
   .NotEmpty().WithMessage("تاريخ التسليم مطلوب")
   .Must(date => date <= DateTime.UtcNow)
   .WithMessage("تاريخ التسليم لا يمكن أن يكون في المستقبل");


            RuleFor(s => s.AwardType)
                .MaximumLength(100).WithMessage("اسم الجائزة طويل جداً.");

            RuleFor(s => s.Status)
    .NotEmpty().WithMessage("حالة التسليم مطلوبة")
    .Must(s => !string.IsNullOrWhiteSpace(s))
    .WithMessage("الحالة لا يمكن أن تكون مسافات فقط")
    .MaximumLength(50);

            RuleFor(s => s.AwardType)
    .NotEmpty()
    .When(s => s.Status == "Approved")
    .WithMessage("يجب تحديد الجائزة عند قبول المشروع");
        }
    }
}