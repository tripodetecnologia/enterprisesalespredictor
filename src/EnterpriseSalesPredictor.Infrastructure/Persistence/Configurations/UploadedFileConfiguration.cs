using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
{
    public void Configure(EntityTypeBuilder<UploadedFile> builder)
    {
        builder.ToTable("uploaded_files");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.FileName)
            .IsRequired()
            .HasMaxLength(260);

        builder.Property(entity => entity.FileType)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(entity => entity.UploadedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(entity => entity.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(entity => entity.UploadedAtUtc)
            .IsRequired();

        builder.HasIndex(entity => entity.UploadedAtUtc);
    }
}
