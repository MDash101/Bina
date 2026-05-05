using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebApplication_School.Models;
namespace WebApplication_School.Models;

public class DiscussionMessage
{
    [Key]
    public int Id { get; set; }

    public int DiscussionId { get; set; }

    public int UserId { get; set; }

    [Required]
    public required string Content { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int? ParentMessageId { get; set; }

    public bool IsEdited { get; set; } = false;
    public bool IsDeleted { get; set; } = false;




    [ForeignKey("UsersId")]
    public virtual Users User { get; set; } = null!;

    [ForeignKey("DiscussionId")]
    public virtual ProjectDiscussions Discussion { get; set; } = null!;
  
    [ForeignKey("ParentMessageId")]
    [JsonIgnore]
    public virtual DiscussionMessages? ParentMessage { get; set; }
    

    public virtual ICollection<DiscussionMessages> Replies { get; set; } = new List<DiscussionMessages>();
}