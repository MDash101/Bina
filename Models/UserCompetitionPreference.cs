using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication_School.Models;
namespace WebApplication_School.Models
{
    public class UserCompetitionPreferences
    {
        [Key]
        public int PreferenceId { get; set; } 

        [Required, StringLength(100)]
        public required string PreferenceName { get; set; }

        [Required, StringLength(50)]
        public required string CompetitionType { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;

        public int CompetitionId { get; set; } 

        [ForeignKey("CompetitionId")]
        public virtual Competitions Competition { get; set; } = null!;
    }
}