using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.Username)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(entity => entity.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(entity => entity.IsActive)
            .IsRequired();

        builder.Property(entity => entity.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(entity => entity.Username)
            .IsUnique();
    }
}
