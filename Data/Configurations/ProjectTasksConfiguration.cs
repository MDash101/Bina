using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class ProjectTasksConfiguration : IEntityTypeConfiguration<ProjectTasks>
{
    public void Configure(EntityTypeBuilder<ProjectTasks> builder)
    {
        builder.HasKey(x => x.TaskId);

    
        builder.HasOne(x => x.Project)
            .WithMany(p => p.ProjectTasks)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

       
        builder.HasOne(x => x.AssignedToUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(x => x.AssignedTo)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(u => u.CreatedTasks)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

       
        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.AssignedTo);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.DueDate);
    }
}