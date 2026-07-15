using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

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

        builder.Property(entity => entity.Zone)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.Address)
            .HasMaxLength(250);

        builder.Property(entity => entity.Phone)
            .HasMaxLength(64);

        builder.HasIndex(entity => entity.Identification)
            .IsUnique();

        builder.HasIndex(entity => entity.City);
    }
}
