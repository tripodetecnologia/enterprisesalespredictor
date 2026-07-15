using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Replenishment;
using EnterpriseSalesPredictor.Application.Validators;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using EnterpriseSalesPredictor.Infrastructure.Replenishment;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EnterpriseSalesPredictor.Tests.Unit.Infrastructure;

public sealed class ReplenishmentServiceTests
{
    [Test]
    public async Task GenerateRecommendationAsync_ShouldPersistRecommendation()
    {
        await using var dbContext = CreateDbContext();
        var productId = SeedReplenishmentData(dbContext);
        var auditLogService = CreateAuditMock();
        var service = new ReplenishmentService(dbContext, auditLogService.Object);

        var result = await service.GenerateRecommendationAsync(new GenerateReplenishmentCommand
        {
            ProductId = productId,
            RequestedBy = "planner"
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.RecommendedUnits, Is.GreaterThan(0));
            Assert.That(result.Status, Is.EqualTo(RecommendationStatus.Pending.ToString()));
            Assert.That(dbContext.ReplenishmentRecommendations.Count(), Is.EqualTo(1));
        });
    }

    [Test]
    public async Task ReviewRecommendationAsync_ShouldApproveForAllowedRole()
    {
        await using var dbContext = CreateDbContext();
        var productId = SeedReplenishmentData(dbContext);
        var auditLogService = CreateAuditMock();
        var service = new ReplenishmentService(dbContext, auditLogService.Object);
        var recommendation = await service.GenerateRecommendationAsync(new GenerateReplenishmentCommand { ProductId = productId, RequestedBy = "planner" });

        var reviewed = await service.ReviewRecommendationAsync(new ReviewReplenishmentCommand
        {
            RecommendationId = recommendation.Id,
            Reviewer = "manager",
            ReviewerRole = "PurchaseManager",
            Action = "approve",
            Notes = "ok"
        });

        Assert.Multiple(() =>
        {
            Assert.That(reviewed.Status, Is.EqualTo(RecommendationStatus.Approved.ToString()));
            Assert.That(reviewed.ReviewedBy, Is.EqualTo("manager"));
        });
    }

    [Test]
    public void ReviewRecommendationAsync_ShouldRejectUnauthorizedRole()
    {
        using var dbContext = CreateDbContext();
        var productId = SeedReplenishmentData(dbContext);
        var auditLogService = CreateAuditMock();
        var service = new ReplenishmentService(dbContext, auditLogService.Object);
        var recommendation = service.GenerateRecommendationAsync(new GenerateReplenishmentCommand { ProductId = productId, RequestedBy = "planner" }).GetAwaiter().GetResult();

        var exception = Assert.ThrowsAsync<ValidationException>(async () => await service.ReviewRecommendationAsync(new ReviewReplenishmentCommand
        {
            RecommendationId = recommendation.Id,
            Reviewer = "seller",
            ReviewerRole = "SalesManager",
            Action = "approve"
        }));

        Assert.That(exception!.Errors.Single().Field, Is.EqualTo(nameof(ReviewReplenishmentCommand.ReviewerRole)));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }

    private static Guid SeedReplenishmentData(AppDbContext dbContext)
    {
        var productId = Guid.NewGuid();
        dbContext.Products.Add(new Product(productId, "Hardware", "Pump", "REF-01", "BrandX", 10m, 18m, 3));

        var customerId = Guid.NewGuid();
        var supplierId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        for (var day = 1; day <= 10; day++)
        {
            dbContext.Sales.Add(new Sale(Guid.NewGuid(), $"INV-{day}", customerId, productId, supplierId, sellerId, DateTime.UtcNow.Date.AddDays(-day), 2m, 20m, "Card"));
        }

        dbContext.SaveChanges();
        return productId;
    }

    private static Mock<IAuditLogService> CreateAuditMock()
    {
        var auditLogService = new Mock<IAuditLogService>();
        auditLogService.Setup(service => service.RecordAsync(It.IsAny<EnterpriseSalesPredictor.Application.Interfaces.Auditing.CreateAuditLogCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EnterpriseSalesPredictor.Application.Interfaces.Auditing.AuditLogDto());
        return auditLogService;
    }
}
