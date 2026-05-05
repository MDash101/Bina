using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class CommentLikesConfiguration : IEntityTypeConfiguration<CommentLikes>
{
    public void Configure(EntityTypeBuilder<CommentLikes> builder)
    {
        builder.HasKey(x => new { x.CommentId, x.UserId });

        builder.HasOne(x => x.Comment)
            .WithMany(c => c.CommentLikes)
            .HasForeignKey(x => x.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(u => u.CommentLikes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}