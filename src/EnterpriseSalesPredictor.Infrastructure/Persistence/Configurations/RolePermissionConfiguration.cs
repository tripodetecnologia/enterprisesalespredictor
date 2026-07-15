using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

        builder.HasKey(entity => entity.Id);

        builder.HasOne(entity => entity.Role)
            .WithMany(role => role.RolePermissions)
            .HasForeignKey(entity => entity.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(entity => entity.Permission)
            .WithMany()
            .HasForeignKey(entity => entity.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(entity => new { entity.RoleId, entity.PermissionId })
            .IsUnique();
    }
}
