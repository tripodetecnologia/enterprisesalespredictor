using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class UploadErrorConfiguration : IEntityTypeConfiguration<UploadError>
{
    public void Configure(EntityTypeBuilder<UploadError> builder)
    {
        builder.ToTable("upload_errors");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.RowNumber)
            .IsRequired();

        builder.Property(entity => entity.FieldName)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.ErrorMessage)
            .IsRequired()
            .HasMaxLength(600);

        builder.HasOne<UploadedFile>()
            .WithMany()
            .HasForeignKey(entity => entity.UploadedFileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(entity => entity.UploadedFileId);
    }
}
