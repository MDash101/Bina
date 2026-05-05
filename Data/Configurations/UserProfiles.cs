using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

namespace WebApplication_School.Data.Configurations
{
    public class UserProfilesConfiguration : IEntityTypeConfiguration<UserProfiles>
    {
        public void Configure(EntityTypeBuilder<UserProfiles> builder)
        {
            builder.HasKey(x => x.ProfileId);

            builder.HasOne(x => x.User)
                .WithOne(u => u.UserProfile)
                .HasForeignKey<UserProfiles>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);
        }
    }
}