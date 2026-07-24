using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Forecasting;
using EnterpriseSalesPredictor.Application.Validators;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Forecasting;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EnterpriseSalesPredictor.Tests.Unit.Infrastructure;

public sealed class ForecastServiceTests
{
    [Test]
    public async Task GenerateForecastAsync_ShouldPersistForecastAndReturnExplanation()
    {
        await using var dbContext = CreateDbContext();
        SeedForecastData(dbContext);
        var auditLogService = new Mock<IAuditLogService>();
        auditLogService.Setup(service => service.RecordAsync(It.IsAny<EnterpriseSalesPredictor.Application.Interfaces.Auditing.CreateAuditLogCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EnterpriseSalesPredictor.Application.Interfaces.Auditing.AuditLogDto());

        var service = new ForecastService(dbContext, auditLogService.Object, new UnitOfWork(dbContext));

        var result = await service.GenerateForecastAsync(new ForecastQuery
        {
            FromDate = new DateTime(2026, 3, 1),
            ToDate = new DateTime(2026, 3, 7),
            RequestedBy = "planner"
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.ProjectedSales, Is.GreaterThanOrEqualTo(0));
            Assert.That(result.Confidence, Is.GreaterThan(0));
            Assert.That(result.Explanation, Does.Contain("promedios diarios"));
            Assert.That(result.CustomerMonthlyForecasts, Is.Not.Empty);
            Assert.That(result.ProductMonthlyForecasts, Is.Not.Empty);
            Assert.That(dbContext.Forecasts.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public void GenerateForecastAsync_ShouldRejectOutOfRangeHorizon()
    {
        using var dbContext = CreateDbContext();
        var auditLogService = new Mock<IAuditLogService>();
        var service = new ForecastService(dbContext, auditLogService.Object, new UnitOfWork(dbContext));

        var exception = Assert.ThrowsAsync<ValidationException>(async () => await service.GenerateForecastAsync(new ForecastQuery
        {
            FromDate = new DateTime(2026, 1, 1),
            ToDate = new DateTime(2027, 2, 1),
            RequestedBy = "planner"
        }));

        Assert.That(exception!.Errors.Single().Field, Is.EqualTo(nameof(ForecastQuery.ToDate)));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }

    private static void SeedForecastData(AppDbContext dbContext)
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        dbContext.Customers.Add(new Customer(customerId, "CLI-01", "Cliente Test", "Bogotá", "Norte", "Calle 1", "5551111"));
        dbContext.Products.Add(new Product(productId, "Hardware", "Producto Test", "REF-01", "BrandX", 10m, 18m, 20));
        dbContext.Suppliers.Add(new Supplier(supplierId, "SUP-01", "Proveedor Test", "Bogotá", "Dirección 1", "5552222"));
        dbContext.Sellers.Add(new Seller(sellerId, "VEN-01", "Vendedor Test"));

        for (var day = 1; day <= 20; day++)
        {
            dbContext.Sales.Add(new Sale(Guid.NewGuid(), $"INV-{day}", customerId, productId, supplierId, sellerId, new DateTime(2026, 2, day), 10m, 100m + day, "Cash"));
        }

        dbContext.SaveChanges();
    }
}
