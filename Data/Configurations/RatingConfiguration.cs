using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(x => x.RatingId);

   
        builder.HasOne(x => x.RatedUser)
            .WithMany(u => u.ReceivedRatings)
            .HasForeignKey(x => x.RatedUserId)
            .OnDelete(DeleteBehavior.Restrict);

     
        builder.HasOne(x => x.RaterUser)
            .WithMany(u => u.GivenRatings)
            .HasForeignKey(x => x.RaterUserId)
            .OnDelete(DeleteBehavior.Restrict);

        
        builder.HasOne(x => x.Project)
            .WithMany(p => p.Ratings)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

       
        builder.HasIndex(x => x.RatedUserId);
        builder.HasIndex(x => x.RaterUserId);
        builder.HasIndex(x => x.ProjectId);
    }
}