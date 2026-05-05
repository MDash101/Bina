using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class ProjectsConfiguration : IEntityTypeConfiguration<Projects>
{
    public void Configure(EntityTypeBuilder<Projects> builder)
    {
        builder.HasKey(x => x.ProjectsId);


        builder.HasOne(x => x.Owner)
            .WithMany(u => u.Projects)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);


        builder.HasOne(x => x.Department)
            .WithMany(d => d.Projects)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

     
        builder.HasOne(x => x.Competition)
            .WithMany(c => c.Projects)
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.SetNull);

    
        builder.HasMany(x => x.ProjectMembers)
            .WithOne(pm => pm.Project)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

       
        builder.HasMany(x => x.ProjectTasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

      
        builder.HasMany(x => x.ProjectMedia)
            .WithOne(m => m.Project)
            .HasForeignKey(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

     
        builder.HasIndex(x => x.OwnerId);
        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.CompetitionId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedDate);

      
        builder.Property(x => x.MilestoneCompletionPercentage)
            .HasPrecision(5, 2);
    }
}