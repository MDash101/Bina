using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class ProjectMediaValidator : AbstractValidator<ProjectMedia>
    {
        public ProjectMediaValidator()
        {
            RuleFor(m => m.FileUrl)
                .NotEmpty().WithMessage("رابط الملف مطلوب")
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage("يجب إدخال رابط (URL) صحيح للصورة أو الفيديو");

            RuleFor(m => m.FileType)
                .IsInEnum()
                .WithMessage("نوع الملف غير صحيح");

            RuleFor(m => m.FileUrl)
    .NotEmpty().WithMessage("رابط الملف مطلوب")
    .Must(u => !string.IsNullOrWhiteSpace(u))
    .WithMessage("الرابط لا يمكن أن يكون مسافات فقط")
    .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
    .WithMessage("يجب إدخال رابط (URL) صحيح للصورة أو الفيديو");

    //        RuleFor(m => m.FileUrl)
    ////.NotEmpty().WithMessage("رابط الملف مطلوب")
    ////.Must(url =>
    ////{
    ////    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
    ////           && (uriResult.Scheme == Uri.UriSchemeHttps
    ////               || uriResult.Scheme == Uri.UriSchemeHttp);
    ////})
    ////.WithMessage("يجب إدخال رابط صحيح يبدأ بـ http أو https");
        }
    }
}