using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class UploadProcessingService : IUploadProcessingService
{
    private readonly AppDbContext _dbContext;

    public UploadProcessingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UploadProcessingResult> ProcessUploadAsync(
        string fileName,
        string fileType,
        string uploadedBy,
        UploadParseResult parseResult,
        CancellationToken cancellationToken = default)
    {
        var upload = new UploadedFile(
            Guid.NewGuid(),
            fileName,
            fileType,
            DateTime.UtcNow,
            uploadedBy,
            UploadProcessStatus.Processing);

        await _dbContext.UploadedFiles.AddAsync(upload, cancellationToken);

        var errors = parseResult.Errors
            .Select(error => new UploadError(Guid.NewGuid(), upload.Id, error.RowNumber, error.FieldName, error.ErrorMessage))
            .ToList();

        var insertedRecords = 0;
        var rowDuplicates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var record in parseResult.Records)
        {
            var duplicateKey = BuildDuplicateKey(record);
            if (!rowDuplicates.Add(duplicateKey))
            {
                errors.Add(new UploadError(Guid.NewGuid(), upload.Id, 0, UploadHeaders.InvoiceNumber, $"Duplicated record in file: {duplicateKey}"));
                continue;
            }

            var alreadyExists = await _dbContext.Sales.AsNoTracking()
                .AnyAsync(sale =>
                    sale.InvoiceNumber == record.InvoiceNumber &&
                    sale.SaleDate.Date == record.SaleDate.Date &&
                    sale.SaleAmount == record.SaleAmount,
                    cancellationToken);

            if (alreadyExists)
            {
                errors.Add(new UploadError(Guid.NewGuid(), upload.Id, 0, UploadHeaders.InvoiceNumber, $"Duplicated record in database: {duplicateKey}"));
                continue;
            }

            var customer = await GetOrCreateCustomerAsync(record, cancellationToken);
            var product = await GetOrCreateProductAsync(record, cancellationToken);
            var supplier = await GetOrCreateSupplierAsync(record, cancellationToken);
            var seller = await GetOrCreateSellerAsync(record, cancellationToken);

            var sale = new Sale(
                Guid.NewGuid(),
                record.InvoiceNumber,
                customer.Id,
                product.Id,
                supplier.Id,
                seller.Id,
                record.SaleDate,
                record.QuantitySold,
                record.SaleAmount,
                record.InvoicePaymentMethod);

            await _dbContext.Sales.AddAsync(sale, cancellationToken);
            insertedRecords++;
        }

        if (errors.Count > 0)
        {
            await _dbContext.UploadErrors.AddRangeAsync(errors, cancellationToken);
        }

        var total = parseResult.Records.Count + parseResult.Errors.Count;
        var invalid = errors.Count;
        var status = invalid > 0 ? UploadProcessStatus.CompletedWithErrors : UploadProcessStatus.Completed;
        upload.Complete(total, insertedRecords, invalid, status);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UploadProcessingResult
        {
            UploadId = upload.Id,
            TotalRecords = total,
            ValidRecords = insertedRecords,
            InvalidRecords = invalid,
            Status = status.ToString()
        };
    }

    private async Task<Customer> GetOrCreateCustomerAsync(UploadRecordData record, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(
            entity => entity.Identification == record.CustomerIdentification,
            cancellationToken);

        if (customer is not null)
        {
            return customer;
        }

        customer = new Customer(
            Guid.NewGuid(),
            record.CustomerIdentification,
            record.CustomerName,
            record.CustomerCity,
            record.CustomerZone,
            record.CustomerAddress,
            record.CustomerPhone);

        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        return customer;
    }

    private async Task<Product> GetOrCreateProductAsync(UploadRecordData record, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(
            entity => entity.Reference == record.ProductReference,
            cancellationToken);

        if (product is not null)
        {
            product.UpdateAvailableUnits(record.ProductAvailableUnits);
            return product;
        }

        product = new Product(
            Guid.NewGuid(),
            record.ProductType,
            record.ProductName,
            record.ProductReference,
            record.ProductBrand,
            record.ProductPurchasePrice,
            record.ProductSalePrice,
            record.ProductAvailableUnits);

        await _dbContext.Products.AddAsync(product, cancellationToken);
        return product;
    }

    private async Task<Supplier> GetOrCreateSupplierAsync(UploadRecordData record, CancellationToken cancellationToken)
    {
        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(
            entity => entity.Identification == record.SupplierIdentification,
            cancellationToken);

        if (supplier is not null)
        {
            return supplier;
        }

        supplier = new Supplier(
            Guid.NewGuid(),
            record.SupplierIdentification,
            record.SupplierName,
            record.SupplierCity,
            record.SupplierAddress,
            record.SupplierPhone);

        await _dbContext.Suppliers.AddAsync(supplier, cancellationToken);
        return supplier;
    }

    private async Task<Seller> GetOrCreateSellerAsync(UploadRecordData record, CancellationToken cancellationToken)
    {
        var seller = await _dbContext.Sellers.FirstOrDefaultAsync(
            entity => entity.Identification == record.SellerIdentification,
            cancellationToken);

        if (seller is not null)
        {
            return seller;
        }

        seller = new Seller(
            Guid.NewGuid(),
            record.SellerIdentification,
            record.SellerName);

        await _dbContext.Sellers.AddAsync(seller, cancellationToken);
        return seller;
    }

    private static string BuildDuplicateKey(UploadRecordData record)
    {
        return string.Join('|',
            record.InvoiceNumber,
            record.ProductReference,
            record.SaleDate.ToString("yyyy-MM-dd"),
            record.SaleAmount.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}
