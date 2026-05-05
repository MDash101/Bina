using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class PrivateMessagesConfiguration : IEntityTypeConfiguration<PrivateMessages>
{
    public void Configure(EntityTypeBuilder<PrivateMessages> builder)
    {
        builder.HasKey(x => x.MessageId);

        builder.HasOne(x => x.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(x => x.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(x => x.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SenderId);
        builder.HasIndex(x => x.ReceiverId);
        builder.HasIndex(x => x.SentDate);
    }
}