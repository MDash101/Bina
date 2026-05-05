using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication_School.Services;
namespace WebApplication_School.Models
{
    public class PrivateMessages
    {
        [Key]
        public int MessageId { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; } = null!;

        public DateTime SentDate { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        [ForeignKey("SenderId")]
        public virtual Users Sender { get; set; } = null!;

        [ForeignKey("ReceiverId")]
        public virtual Users Receiver { get; set; } = null!;

      
      
    }
}