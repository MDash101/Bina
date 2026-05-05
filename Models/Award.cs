using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication_School.Models;
namespace WebApplication_School.Models
{
    public class Awards
    {
        [Key]
        public int AwardsId { get; set; }


        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Type { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

       public DateTime AwardedDate { get; set; } = DateTime.UtcNow;

        [StringLength(255)]
        public string? BadgeUrl { get; set; }

        public int PointsAwarded { get; set; }

        [ForeignKey("UserId")]
        public virtual Users User { get; set; } = null!;
    }
}