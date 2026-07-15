using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Forecasting;
using EnterpriseSalesPredictor.Application.Validators;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Forecasting;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
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

        var service = new ForecastService(dbContext, auditLogService.Object);

        var result = await service.GenerateForecastAsync(new ForecastQuery
        {
            FromDate = new DateTime(2026, 3, 1),
            ToDate = new DateTime(2026, 3, 7),
            RequestedBy = "planner"
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.ProjectedSales, Is.GreaterThan(0));
            Assert.That(result.Confidence, Is.GreaterThan(0));
            Assert.That(result.Explanation, Does.Contain("average daily sales"));
            Assert.That(dbContext.Forecasts.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public void GenerateForecastAsync_ShouldRejectOutOfRangeHorizon()
    {
        using var dbContext = CreateDbContext();
        var auditLogService = new Mock<IAuditLogService>();
        var service = new ForecastService(dbContext, auditLogService.Object);

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

        for (var day = 1; day <= 20; day++)
        {
            dbContext.Sales.Add(new Sale(Guid.NewGuid(), $"INV-{day}", customerId, productId, supplierId, sellerId, new DateTime(2026, 2, day), 10m, 100m + day, "Cash"));
        }

        dbContext.SaveChanges();
    }
}
