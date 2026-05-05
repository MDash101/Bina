using System.ComponentModel.DataAnnotations;

namespace WebApplication_School.DTOs
{
    public class RegisterDto
    {
        [Required, StringLength(255)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = null!;

        [Required]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "يجب إدخال أرقام فقط")]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "طول الرقم غير منطقي")]
        public string PhoneNumber { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? Address { get; set; }
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}