using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication_School.Models
{
    [Authorize]
    [ApiController] 
    [Route("api/[controller]")] 
    public class DiscussionMessages
    {
        [Key]
        public int Id { get; set; }

        public int DiscussionId { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = null!;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int? ParentMessageId { get; set; }

        public bool IsEdited { get; set; } = false;
        public bool IsDeleted { get; set; } = false;

    

        [ForeignKey("UserId")]
        public virtual Users? User { get; set; }

        [ForeignKey("DiscussionId")]
        public virtual ProjectDiscussions Discussion { get; set; } = null!;

        [ForeignKey("ParentMessageId")]
        [JsonIgnore]
        public virtual DiscussionMessages? ParentMessage { get; set; }

        [JsonIgnore]
        public virtual ICollection<DiscussionMessages> Replies { get; set; }
            = new List<DiscussionMessages>();
    }
}