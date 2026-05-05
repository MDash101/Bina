using Microsoft.EntityFrameworkCore;
using WebApplication_School.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
namespace WebApplication_School.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserProfiles> UserProfiles { get; set; }
        public DbSet<Skills> Skills { get; set; }
        public DbSet<UserSkills> UserSkills { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<ProjectMembers> ProjectMembers { get; set; }
        public DbSet<ProjectMedia> ProjectMedia { get; set; }
        public DbSet<ProjectDiscussions> ProjectDiscussions { get; set; }
        public DbSet<DiscussionMessages> DiscussionMessages { get; set; }
        public DbSet<ProjectTasks> ProjectTasks { get; set; }
        public DbSet<ProjectSkills> ProjectSkills { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<CommentLikes> CommentLikes { get; set; }
        public DbSet<PrivateMessages> PrivateMessages { get; set; }
        public DbSet<Notifications> Notifications { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Competitions> Competitions { get; set; }
        public DbSet<CompetitionDetails> CompetitionDetails { get; set; }
        public DbSet<CompetitionSubmissions> CompetitionSubmissions { get; set; }
        public DbSet<UserCompetitionPreferences> UserCompetitionPreferences { get; set; }
        public DbSet<FileStorage> FileStorages { get; set; }
        public DbSet<Awards> Awards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

          
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            foreach (var entry in ChangeTracker.Entries())
            {
              
                if (entry.Entity is AuditLog || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                
                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = userId,
                    Action = entry.State.ToString()
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue!;
                            break;
                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue!;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue!;
                                auditEntry.NewValues[propertyName] = property.CurrentValue!;
                            }
                            break;
                    }
                }
            }
            return auditEntries;
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
            return base.SaveChangesAsync();
        }
    }
}