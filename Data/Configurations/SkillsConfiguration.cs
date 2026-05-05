using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class SkillsConfiguration : IEntityTypeConfiguration<Skills>
{
    public void Configure(EntityTypeBuilder<Skills> builder)
    {
        builder.HasKey(x => x.SkillId);

        builder.HasOne(x => x.Department)
            .WithMany(d => d.Skills)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.Name);
    }
}