using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class CompetitionDetailsConfiguration : IEntityTypeConfiguration<CompetitionDetails>
{
    public void Configure(EntityTypeBuilder<CompetitionDetails> builder)
    {
        builder.HasKey(x => x.CompetitionDetailId);

        builder.HasOne(x => x.Competition)
            .WithOne(c => c.CompetitionDetail)
            .HasForeignKey<CompetitionDetails>(x => x.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}