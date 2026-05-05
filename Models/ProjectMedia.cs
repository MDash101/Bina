using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class ProjectMedia
    {
        public enum MediaType
        {
            Image = 0,
            Video = 1,
            File = 2,
            Pdf = 3
        }

        [Key]
        public int MediaId { get; set; }

        public int ProjectId { get; set; }

        [Required, StringLength(255)]
        public string FileName { get; set; } = null!;

        [Required, StringLength(500)]
        public string FileUrl { get; set; } = null!;

        public MediaType FileType { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string Description { get; set; } = null!;

        public bool IsPrimary { get; set; } = false;

        public long FileSize { get; set; }

        [StringLength(500)]
        public string? ThumbnailUrl { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; } = null!;
    }
}