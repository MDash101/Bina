using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class ProjectMembersConfiguration : IEntityTypeConfiguration<ProjectMembers>
{
    public void Configure(EntityTypeBuilder<ProjectMembers> builder)
    {
        builder.HasKey(x => x.ProjectMemberId);

        builder.HasOne(x => x.Project)
            .WithMany(p => p.ProjectMembers)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(u => u.ProjectMembers)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProjectId);
        builder.HasIndex(x => x.UserId);

       
        builder.HasIndex(x => new { x.ProjectId, x.UserId })
            .IsUnique();
    }
}