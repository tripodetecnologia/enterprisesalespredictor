using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(entity => entity.SaleDate)
            .IsRequired();

        builder.Property(entity => entity.Quantity)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(entity => entity.SaleAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(entity => entity.PaymentMethod)
            .IsRequired()
            .HasMaxLength(80);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(entity => entity.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(entity => entity.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Supplier>()
            .WithMany()
            .HasForeignKey(entity => entity.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Seller>()
            .WithMany()
            .HasForeignKey(entity => entity.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => entity.SaleDate);
        builder.HasIndex(entity => new { entity.CustomerId, entity.SaleDate });
        builder.HasIndex(entity => new { entity.ProductId, entity.SaleDate });
        builder.HasIndex(entity => new { entity.SupplierId, entity.SaleDate });
        builder.HasIndex(entity => new { entity.SellerId, entity.SaleDate });
    }
}
