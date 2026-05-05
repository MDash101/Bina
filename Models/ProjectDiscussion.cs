using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class ProjectDiscussions
    {
        [Key]
        public int DiscussionId { get; set; }

        public int ProjectId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsPinned { get; set; } = false;

        public DateTime? LastActivityDate { get; set; }

        public int CreatedById { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; } = null!;

        [ForeignKey("CreatedById")]
        public virtual Users CreatedBy { get; set; } = null!;

        public virtual ICollection<DiscussionMessages> Messages { get; set; } = new List<DiscussionMessages>();
    }
}