using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Users
    {
        [Key]
        public int UsersId { get; set; }

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = null!;

        [Required, StringLength(255)]
        public string PasswordHash { get; set; } = null!;

        [StringLength(100)]
        public UserRole Role { get; set; } = UserRole.Student;

        public int? DepartmentId { get; set; }

        [Required, StringLength(255)]
        public string FullName { get; set; } = null!;

        public bool IsEmailVerified { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; }

        public virtual ICollection<Projects> Projects { get; set; } = new List<Projects>();
        public virtual ICollection<UserSkills> UserSkills { get; set; } = new List<UserSkills>();
        public UserProfiles UserProfile { get; set; } = null!;

        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

        public ICollection<Awards> Awards { get; set; } = new List<Awards>();
        public ICollection<Comments> Comments { get; set; } = new List<Comments>();
        public ICollection<CommentLikes> CommentLikes { get; set; } = new List<CommentLikes>();
        public ICollection<DiscussionMessages> DiscussionMessages { get; set; } = new List<DiscussionMessages>();
        public ICollection<FileStorage> UploadedFiles { get; set; } = new List<FileStorage>();
        public ICollection<PrivateMessages> SentMessages { get; set; } = new List<PrivateMessages>();
        public ICollection<ProjectMembers> ProjectMembers { get; set; } = new List<ProjectMembers>();
        public ICollection<PrivateMessages> ReceivedMessages { get; set; } = new List<PrivateMessages>();
        public ICollection<Notifications> Notifications { get; set; } = new List<Notifications>();
        public ICollection<ProjectDiscussions> CreatedDiscussions { get; set; } = new List<ProjectDiscussions>();
        public ICollection<ProjectTasks> AssignedTasks { get; set; } = new List<ProjectTasks>();
        public ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();
        

        public ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();
        public ICollection<ProjectTasks> CreatedTasks { get; set; } = new List<ProjectTasks>();
        public ICollection<UserCompetitionPreferences> UserCompetitionPreferences { get; set; } = new List<UserCompetitionPreferences>();
    }
}