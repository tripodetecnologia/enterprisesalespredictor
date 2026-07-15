using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("sellers");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Identification)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(entity => entity.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(entity => entity.Identification)
            .IsUnique();
    }
}
