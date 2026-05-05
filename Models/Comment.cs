using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Comments
    {
        [Key]
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public int UserId { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public int? ParentCommentId { get; set; }

        public bool IsDeleted { get; set; } = false;
        public bool IsEdited { get; set; } = false;
        public int LikeCount { get; set; } = 0;

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        [ForeignKey("ParentCommentId")]
        public virtual Comments? ParentComment { get; set; }
        
        public ICollection<CommentLikes> CommentLikes { get; set; } = new List<CommentLikes>();
        public virtual ICollection<Comments> Replies { get; set; } = new List<Comments>();
    }
}