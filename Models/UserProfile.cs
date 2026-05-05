using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{


    public enum UserRole
    {
        [Display(Name = "طالب")]
        Student = 1,

        [Display(Name = "معلم")]
        Lecturer = 2,

        [Display(Name = "مسؤول")]
        Admin = 3
    }

    public enum UserStatus
    {
        [Display(Name = "نشط")]
        Active = 1,

        [Display(Name = "غير نشط")]
        Inactive = 2,

        [Display(Name = "مشغول")]
        Busy = 3
    }



    public class UserProfiles
    {
        [Key]
        public int ProfileId { get; set; } 

        public int UserId { get; set; } 

        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(255)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        [StringLength(255)]
        public required string LastName { get; set; }

        public string? Bio { get; set; }

        [Url]
        public string? AvatarUrl { get; set; }

        [Required, StringLength(100)]
        public required string AcademicYear { get; set; }

        [Required]
        public required string SkillsSummary { get; set; }

        public string? GitHubUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Country { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public int Age { get; set; }

        public bool IsPublic { get; set; } = false;

      
        public int CompetitionsCount { get; set; } = 0;
        public int ProjectsCount { get; set; } = 0;
        public int AwardsCount { get; set; } = 0;

        public int TotalPoints { get; set; } = 0;
        public UserRole Role { get; set; } = UserRole.Student;
        public UserStatus Status { get; set; } = UserStatus.Active;

     

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

      
        public virtual ICollection<UserSkills> Skills { get; set; } = new List<UserSkills>();
    }
}