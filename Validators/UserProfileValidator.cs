using FluentValidation;
using WebApplication_School.Models;

namespace WebApplication_School.Validators
{
    public class UserProfileValidator : AbstractValidator<UserProfiles>
    {
        public UserProfileValidator()
        {
            RuleFor(p => p.FirstName)
.NotEmpty().WithMessage("الاسم الأول مطلوب")
.Must(n => !string.IsNullOrWhiteSpace(n))
.WithMessage("الاسم الأول لا يمكن أن يكون مسافات فقط")
.MaximumLength(50);

            RuleFor(p => p.LastName)
    .NotEmpty().WithMessage("اسم العائلة مطلوب")
    .Must(n => !string.IsNullOrWhiteSpace(n))
    .WithMessage("اسم العائلة لا يمكن أن يكون مسافات فقط");

            RuleFor(p => p.GitHubUrl)
     .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
     .WithMessage("رابط GitHub غير صحيح");

            RuleFor(p => p.LinkedInUrl)
         .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
         .WithMessage("رابط LinkedIn غير صحيح");

            RuleFor(p => p.Age)
              
                .InclusiveBetween(15, 80).WithMessage("العمر يجب أن يكون منطقياً (بين 15 و 80 سنة)");

            RuleFor(p => p.Bio)
      .Must(b => string.IsNullOrEmpty(b) || !string.IsNullOrWhiteSpace(b))
      .WithMessage("النبذة لا يمكن أن تكون مسافات فقط")
      .MaximumLength(500);

        }
    }
}