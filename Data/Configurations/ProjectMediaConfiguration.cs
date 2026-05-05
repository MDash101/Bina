using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication_School.Models;

public class ProjectMediaConfiguration : IEntityTypeConfiguration<ProjectMedia>
{
    public void Configure(EntityTypeBuilder<ProjectMedia> builder)
    {
        builder.HasKey(x => x.MediaId);

        builder.HasOne(x => x.Project)
            .WithMany(p => p.ProjectMedia)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ProjectId);
    }
}