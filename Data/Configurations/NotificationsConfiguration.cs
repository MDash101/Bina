using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class NotificationsConfiguration : IEntityTypeConfiguration<Notifications>
{
    public void Configure(EntityTypeBuilder<Notifications> builder)
    {
        builder.HasKey(x => x.NotificationId);

        builder.HasOne(x => x.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.IsRead);
        builder.HasIndex(x => x.CreatedDate);
    }
}