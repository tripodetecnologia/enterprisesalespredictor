using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Identification)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(entity => entity.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entity => entity.City)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.Address)
            .HasMaxLength(250);

        builder.Property(entity => entity.Phone)
            .HasMaxLength(64);

        builder.HasIndex(entity => entity.Identification)
            .IsUnique();
    }
}
