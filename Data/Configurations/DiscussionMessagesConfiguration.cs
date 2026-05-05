using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class DiscussionMessageConfiguration : IEntityTypeConfiguration<DiscussionMessages>
{
    public void Configure(EntityTypeBuilder<DiscussionMessages> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User)
            .WithMany(u => u.DiscussionMessages)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Discussion)
            .WithMany(d => d.Messages)
            .HasForeignKey(x => x.DiscussionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ParentMessage)
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.ParentMessageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}