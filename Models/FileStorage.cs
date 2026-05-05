using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Formats.Tar;

namespace WebApplication_School.Models
{
    public class FileStorage
    {
        [Key]
        public int FileId { get; set; }

        [Required, StringLength(255)]
        public string FileName { get; set; } = null!;

        [Required, StringLength(500)]
        public string FilePath { get; set; } = null!;
        
        public int FileSize { get; set; }

        [Required, StringLength(100)]
        public string FileType { get; set; } = null!;
        public int UploaderId { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public bool IsPublic { get; set; } = false;

        public int DownloadCount { get; set; } = 0;

        public string? Description { get; set; }
        public string? Category { get; set; }

        [ForeignKey("UploaderId")]
        public  Users Uploader { get; set; } = null!;
    }
}