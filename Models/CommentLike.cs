using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class CommentLikes
    {
        [Key, Column(Order = 0)]
        public int CommentId { get; set; }

        [Key, Column(Order = 1)]
        public int UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("CommentId")]
        public virtual Comments Comment { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}