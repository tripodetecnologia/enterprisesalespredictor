using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class ReplenishmentRecommendationConfiguration : IEntityTypeConfiguration<ReplenishmentRecommendation>
{
    public void Configure(EntityTypeBuilder<ReplenishmentRecommendation> builder)
    {
        builder.ToTable("replenishment_recommendations");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.GeneratedAtUtc)
            .IsRequired();

        builder.Property(entity => entity.RecommendedUnits)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(entity => entity.Confidence)
            .HasPrecision(5, 4)
            .IsRequired();

        builder.Property(entity => entity.Rationale)
            .IsRequired()
            .HasMaxLength(1200);

        builder.Property(entity => entity.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(entity => entity.ReviewedBy)
            .HasMaxLength(120);

        builder.Property(entity => entity.ReviewNotes)
            .HasMaxLength(600);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(entity => entity.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entity => new { entity.ProductId, entity.GeneratedAtUtc });
        builder.HasIndex(entity => entity.Status);
    }
}
