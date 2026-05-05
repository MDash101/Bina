using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class FileStorageValidator : AbstractValidator<FileStorage>
    {
        public FileStorageValidator()
        {
            RuleFor(f => f.FileName)
                .NotEmpty().WithMessage("اسم الملف مطلوب");

            RuleFor(f => f.FilePath)
                .NotEmpty().WithMessage("مسار الملف مطلوب");

            RuleFor(f => f.FileSize)
                .ExclusiveBetween(0, 52428800).WithMessage("حجم الملف يجب أن يكون أكبر من صفر وأقل من 50 ميجابايت");

            RuleFor(f => f.FileType)
                .NotEmpty().WithMessage("نوع الملف مطلوب (pdf, zip, docx, etc.)");

            RuleFor(f => f.FileName)
    .NotEmpty().WithMessage("اسم الملف مطلوب")
    .Must(n => !string.IsNullOrWhiteSpace(n))
    .WithMessage("اسم الملف غير صالح");

            RuleFor(f => f.FilePath)
                .NotEmpty().WithMessage("مسار الملف مطلوب")
                .Must(p => !string.IsNullOrWhiteSpace(p))
                .WithMessage("مسار الملف غير صالح");

            RuleFor(f => f.FileType)
            .NotEmpty()
            .Must(t => new[]
            {
        
        "pdf", "doc", "docx", "ppt", "pptx", "xls", "xlsx",     
        "zip", "rar", "7z",      
        "png", "jpg", "jpeg", "gif",        
        "cs", "js", "ts", "html", "css",
        "py", "java", "cpp", "c", "php"

            }.Contains(t.ToLower()))
            .WithMessage("نوع الملف غير مدعوم");

            RuleFor(f => f.FileName)
    .Matches(@"^[^\\/:*?""<>|]+$")
    .WithMessage("اسم الملف يحتوي على رموز غير مسموحة");
        }
    }
}