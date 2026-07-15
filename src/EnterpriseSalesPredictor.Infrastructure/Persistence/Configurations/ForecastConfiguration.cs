using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class ForecastConfiguration : IEntityTypeConfiguration<Forecast>
{
    public void Configure(EntityTypeBuilder<Forecast> builder)
    {
        builder.ToTable("forecasts");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.GeneratedAtUtc)
            .IsRequired();

        builder.Property(entity => entity.FromDate)
            .IsRequired();

        builder.Property(entity => entity.ToDate)
            .IsRequired();

        builder.Property(entity => entity.ProjectedSales)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(entity => entity.Confidence)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(entity => entity.GeneratedBy)
            .IsRequired()
            .HasMaxLength(120);

        builder.HasIndex(entity => entity.GeneratedAtUtc);
    }
}
