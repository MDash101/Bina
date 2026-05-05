using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? HeadId { get; set; }

        [ForeignKey("HeadId")]
        public virtual Users? Head { get; set; }

        [EmailAddress, StringLength(255)]
        public string? ContactEmail { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<Competitions> Competitions { get; set; } = new List<Competitions>();
        public virtual ICollection<Users> Users { get; set; } = new List<Users>();
        public virtual ICollection<Projects> Projects { get; set; } = new List<Projects>();
        public virtual ICollection<Skills> Skills { get; set; } = new List<Skills>();
    }
}