using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class ProjectSkillsConfiguration : IEntityTypeConfiguration<ProjectSkills>
{
    public void Configure(EntityTypeBuilder<ProjectSkills> entity)
    {
        entity.HasKey(x => x.ProjectSkillId);

        entity.HasOne(x => x.Project)
            .WithMany(p => p.ProjectSkills)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Skill)
            .WithMany(s => s.ProjectSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.FilledByUser)
            .WithMany()
            .HasForeignKey(x => x.FilledByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        
        entity.HasIndex(x => new { x.ProjectId, x.SkillId })
              .IsUnique();
    }
}