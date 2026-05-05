using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class ProjectMembers
    {
        [Key]
        public int ProjectMemberId { get; set; }

        public int ProjectId { get; set; }

        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string Role { get; set; } = null!;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [Required, StringLength(50)]
        public string Status { get; set; } = "Active";

        public string? Permissions { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}