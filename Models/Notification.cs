using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Notifications
    {
        [Key]
        public int NotificationId { get; set; }

        public int UserId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        [Required, StringLength(50)]
        public string Type { get; set; } = null!;

        public int? RelatedId { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}