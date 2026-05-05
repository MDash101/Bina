namespace WebApplication_School.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public Users? User { get; set; }

        public string? RecordId { get; set; }
        public string TableName { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
    }
}
