using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.ProductType)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(entity => entity.Reference)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(entity => entity.Brand)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.PurchasePrice)
            .HasPrecision(18, 2);

        builder.Property(entity => entity.SalePrice)
            .HasPrecision(18, 2);

        builder.Property(entity => entity.AvailableUnits)
            .IsRequired();

        builder.HasIndex(entity => entity.Reference)
            .IsUnique();
    }
}
