using EnterpriseSalesPredictor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Seller> Sellers => Set<Seller>();

    public DbSet<Sale> Sales => Set<Sale>();

    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();

    public DbSet<UploadError> UploadErrors => Set<UploadError>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    public DbSet<Forecast> Forecasts => Set<Forecast>();

    public DbSet<ReplenishmentRecommendation> ReplenishmentRecommendations => Set<ReplenishmentRecommendation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
