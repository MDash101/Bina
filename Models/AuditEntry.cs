using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using WebApplication_School.Models;

namespace WebApplication_School.Data
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string? UserId { get; set; }
        public string TableName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

        public AuditLog ToAudit()
        {
            var audit = new AuditLog();

            
            if (int.TryParse(UserId, out int parsedUserId))
            {
                audit.UserId = parsedUserId;
            }
            else
            {
                audit.UserId = null;
            }

            audit.TableName = TableName;
            audit.Action = Action;
            audit.CreatedAt = DateTime.UtcNow; 

           
            audit.RecordId = JsonConvert.SerializeObject(KeyValues);

            
            audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
            audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);

            return audit;
        }
    }
}