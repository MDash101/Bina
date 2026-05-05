using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class CompetitionsConfiguration : IEntityTypeConfiguration<Competitions>
{
    public void Configure(EntityTypeBuilder<Competitions> builder)
    {
        builder.HasKey(x => x.CompetitionId);

        builder.HasOne(x => x.Department)
            .WithMany(d => d.Competitions)
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Projects)
            .WithOne(p => p.Competition)
            .HasForeignKey(p => p.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.DepartmentId);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.StartDate);
    }
}