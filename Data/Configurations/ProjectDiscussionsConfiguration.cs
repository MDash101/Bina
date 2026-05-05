using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class ProjectDiscussionsConfiguration : IEntityTypeConfiguration<ProjectDiscussions>
{
    public void Configure(EntityTypeBuilder<ProjectDiscussions> builder)
    {
        builder.HasKey(x => x.DiscussionId);

        builder.HasOne(x => x.Project)
            .WithMany(p => p.ProjectDiscussions)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.CreatedBy)
            .WithMany(u => u.CreatedDiscussions)
            .HasForeignKey(x => x.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Messages)
            .WithOne(m => m.Discussion)
            .HasForeignKey(m => m.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}