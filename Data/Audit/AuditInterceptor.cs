using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using WebApplication_School.Models;

namespace WebApplication_School.Data { 
public class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditLogs(DbContext? context)
    {
        if (context == null) return;

        
        if (context.ChangeTracker.Entries<AuditLog>().Any())
            return;

        var auditEntries = new List<AuditLog>();

        var entries = context.ChangeTracker.Entries()
            .Where(e =>
                e.Entity is not AuditLog &&
                e.State != EntityState.Unchanged &&
                e.State != EntityState.Detached)
            .ToList();

        foreach (var entry in entries)
        {
            var audit = new AuditLog
            {
                TableName = entry.Entity.GetType().Name,
                Action = entry.State.ToString(),
                CreatedAt = DateTime.UtcNow
            };

          
            var key = entry.Properties
                .FirstOrDefault(p => p.Metadata.IsPrimaryKey());

            audit.RecordId = key?.CurrentValue?.ToString();

           
            if (entry.State == EntityState.Added)
            {
                audit.NewValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
            }

            if (entry.State == EntityState.Modified)
            {
                audit.OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
                audit.NewValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject());
            }

            
            if (entry.State == EntityState.Deleted)
            {
                audit.OldValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject());
            }

            auditEntries.Add(audit);
        }

        if (auditEntries.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditEntries);
        }
    }
}
}