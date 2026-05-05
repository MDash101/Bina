using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

namespace WebApplication_School.Data.Configurations
{
    public class UserCompetitionPreferencesConfiguration : IEntityTypeConfiguration<UserCompetitionPreferences>
    {
        public void Configure(EntityTypeBuilder<UserCompetitionPreferences> builder)
        {
            builder.HasKey(x => x.PreferenceId);

            builder.HasOne(x => x.User)
                .WithMany(u => u.UserCompetitionPreferences)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserId);
        }
    }
}