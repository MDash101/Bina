using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class AwardsConfiguration : IEntityTypeConfiguration<Awards>
{
    public void Configure(EntityTypeBuilder<Awards> builder)
    {
        builder.HasKey(x => x.AwardsId);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Awards)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}