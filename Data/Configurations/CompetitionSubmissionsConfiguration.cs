using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class CompetitionSubmissionsConfiguration : IEntityTypeConfiguration<CompetitionSubmissions>
{
    public void Configure(EntityTypeBuilder<CompetitionSubmissions> builder)
    {
        builder.HasKey(x => x.SubmissionId);

        builder.HasOne(x => x.Competition)
            .WithMany(c => c.CompetitionSubmissions)
            .HasForeignKey(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Project)
            .WithMany(p => p.CompetitionSubmissions)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}