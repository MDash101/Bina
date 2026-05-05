using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class FileStorageConfiguration : IEntityTypeConfiguration<FileStorage>
{
    public void Configure(EntityTypeBuilder<FileStorage> builder)
    {
        builder.HasKey(x => x.FileId);

        builder.HasOne(x => x.Uploader)
            .WithMany(u => u.UploadedFiles)
            .HasForeignKey(x => x.UploaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UploaderId);
        builder.HasIndex(x => x.UploadDate);
        builder.HasIndex(x => x.FileType);
    }
}