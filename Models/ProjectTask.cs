using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication_School.Models
{
    public enum ProjectTaskStatus
    {
        Todo = 0,
        InProgress = 1,
        Completed = 2,
        OnHold = 3
    }

    public class ProjectTasks
    {
        [Key]
        public int TaskId { get; set; }

        public int ProjectId { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public int? AssignedTo { get; set; }

        public int CreatedBy { get; set; }

        public ProjectTaskStatus Status { get; set; }

        public DateTime? DueDate { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Low;
        public enum TaskPriority
        {
            Low = 1,
            Medium = 2,
            High = 3,
            Urgent = 4,
            Critical = 5
        }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; } = null!;

        [ForeignKey("AssignedTo")]
        public virtual Users? AssignedToUser { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual Users CreatedByUser { get; set; } = null!;
    }
}