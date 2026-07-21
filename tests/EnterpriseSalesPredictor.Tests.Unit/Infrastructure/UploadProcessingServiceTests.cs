using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using EnterpriseSalesPredictor.Infrastructure.FileProcessing;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Tests.Unit.Infrastructure;

public sealed class UploadProcessingServiceTests
{
    [Test]
    public async Task ProcessUploadAsync_ShouldReuseTrackedEntitiesWithinSameBatch()
    {
        await using var dbContext = CreateDbContext();
        var service = new UploadProcessingService(dbContext);

        var parseResult = new UploadParseResult();
        parseResult.Records.Add(CreateRecord("INV-001", new DateTime(2026, 4, 1), 120m));
        parseResult.Records.Add(CreateRecord("INV-002", new DateTime(2026, 4, 2), 140m));

        var result = await service.ProcessUploadAsync("sample.xlsx", "excel", "tester", parseResult);

        Assert.Multiple(() =>
        {
            Assert.That(result.ValidRecords, Is.EqualTo(2));
            Assert.That(dbContext.Customers.Count(), Is.EqualTo(1));
            Assert.That(dbContext.Products.Count(), Is.EqualTo(1));
            Assert.That(dbContext.Suppliers.Count(), Is.EqualTo(1));
            Assert.That(dbContext.Sellers.Count(), Is.EqualTo(1));
            Assert.That(dbContext.Sales.Count(), Is.EqualTo(2));
        });
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }

    private static UploadRecordData CreateRecord(string invoiceNumber, DateTime saleDate, decimal saleAmount)
    {
        return new UploadRecordData
        {
            InvoiceNumber = invoiceNumber,
            CustomerName = "Client A",
            CustomerIdentification = "CLI-01",
            CustomerAddress = "Main St",
            CustomerCity = "Quito",
            CustomerPhone = "555-111",
            CustomerZone = "North",
            ProductType = "Hardware",
            ProductName = "Valve",
            ProductReference = "VAL-01",
            ProductBrand = "BrandX",
            ProductPurchasePrice = 10m,
            ProductSalePrice = 15m,
            ProductAvailableUnits = 20,
            QuantitySold = 3m,
            SaleAmount = saleAmount,
            SaleDate = saleDate,
            SellerName = "Seller One",
            SellerIdentification = "VEN-01",
            SupplierName = "Supplier One",
            SupplierIdentification = "SUP-01",
            SupplierAddress = "Industrial Av",
            SupplierPhone = "555-222",
            SupplierCity = "Quito",
            InvoicePaymentMethod = "Cash"
        };
    }
}
