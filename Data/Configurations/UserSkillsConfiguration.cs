using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class UserSkillsConfiguration : IEntityTypeConfiguration<UserSkills>
{
    public void Configure(EntityTypeBuilder<UserSkills> builder)
    {
        builder.HasKey(x => x.UserSkillId);

        builder.HasOne(x => x.User)
            .WithMany(u => u.UserSkills)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Skill)
            .WithMany(s => s.UserSkills)
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new { x.UserId, x.SkillId })
              .IsUnique();
    }
}