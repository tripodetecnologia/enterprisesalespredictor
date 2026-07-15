using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.OccurredAtUtc)
            .IsRequired();

        builder.Property(entity => entity.Actor)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.Action)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.Module)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.Details)
            .IsRequired()
            .HasMaxLength(1200);

        builder.HasIndex(entity => entity.OccurredAtUtc);
        builder.HasIndex(entity => entity.Module);
    }
}
