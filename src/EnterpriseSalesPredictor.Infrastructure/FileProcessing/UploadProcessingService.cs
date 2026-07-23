using EnterpriseSalesPredictor.Application.Constants;
using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
        var customerCache = await LoadCustomersAsync(parseResult.Records, cancellationToken);
        var productCache = await LoadProductsAsync(parseResult.Records, cancellationToken);
        var supplierCache = await LoadSuppliersAsync(parseResult.Records, cancellationToken);
        var sellerCache = await LoadSellersAsync(parseResult.Records, cancellationToken);
        var existingSales = await LoadExistingSalesAsync(parseResult.Records, cancellationToken);

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

            if (existingSales.Contains(duplicateKey))
            {
                errors.Add(new UploadError(Guid.NewGuid(), upload.Id, 0, UploadHeaders.InvoiceNumber, $"Duplicated record in database: {duplicateKey}"));
                continue;
            }

            var customer = await GetOrCreateCustomerAsync(record, customerCache, cancellationToken);
            var product = await GetOrCreateProductAsync(record, productCache, cancellationToken);
            var supplier = await GetOrCreateSupplierAsync(record, supplierCache, cancellationToken);
            var seller = await GetOrCreateSellerAsync(record, sellerCache, cancellationToken);

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
            existingSales.Add(duplicateKey);
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

    private async Task<Customer> GetOrCreateCustomerAsync(
        UploadRecordData record,
        IDictionary<string, Customer> customerCache,
        CancellationToken cancellationToken)
    {
        if (customerCache.TryGetValue(record.CustomerIdentification, out var cachedCustomer))
        {
            return cachedCustomer;
        }

        var localCustomer = _dbContext.Customers.Local.FirstOrDefault(entity =>
            entity.Identification.Equals(record.CustomerIdentification, StringComparison.OrdinalIgnoreCase));
        if (localCustomer is not null)
        {
            customerCache[record.CustomerIdentification] = localCustomer;
            return localCustomer;
        }

        var customer = await _dbContext.Customers.FirstOrDefaultAsync(
            entity => entity.Identification == record.CustomerIdentification,
            cancellationToken);

        if (customer is not null)
        {
            customerCache[record.CustomerIdentification] = customer;
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
        customerCache[record.CustomerIdentification] = customer;
        return customer;
    }

    private async Task<Product> GetOrCreateProductAsync(
        UploadRecordData record,
        IDictionary<string, Product> productCache,
        CancellationToken cancellationToken)
    {
        if (productCache.TryGetValue(record.ProductReference, out var cachedProduct))
        {
            cachedProduct.UpdateAvailableUnits(record.ProductAvailableUnits);
            return cachedProduct;
        }

        var localProduct = _dbContext.Products.Local.FirstOrDefault(entity =>
            entity.Reference.Equals(record.ProductReference, StringComparison.OrdinalIgnoreCase));
        if (localProduct is not null)
        {
            localProduct.UpdateAvailableUnits(record.ProductAvailableUnits);
            productCache[record.ProductReference] = localProduct;
            return localProduct;
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(
            entity => entity.Reference == record.ProductReference,
            cancellationToken);

        if (product is not null)
        {
            product.UpdateAvailableUnits(record.ProductAvailableUnits);
            productCache[record.ProductReference] = product;
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
        productCache[record.ProductReference] = product;
        return product;
    }

    private async Task<Supplier> GetOrCreateSupplierAsync(
        UploadRecordData record,
        IDictionary<string, Supplier> supplierCache,
        CancellationToken cancellationToken)
    {
        if (supplierCache.TryGetValue(record.SupplierIdentification, out var cachedSupplier))
        {
            return cachedSupplier;
        }

        var localSupplier = _dbContext.Suppliers.Local.FirstOrDefault(entity =>
            entity.Identification.Equals(record.SupplierIdentification, StringComparison.OrdinalIgnoreCase));
        if (localSupplier is not null)
        {
            supplierCache[record.SupplierIdentification] = localSupplier;
            return localSupplier;
        }

        var supplier = await _dbContext.Suppliers.FirstOrDefaultAsync(
            entity => entity.Identification == record.SupplierIdentification,
            cancellationToken);

        if (supplier is not null)
        {
            supplierCache[record.SupplierIdentification] = supplier;
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
        supplierCache[record.SupplierIdentification] = supplier;
        return supplier;
    }

    private async Task<Seller> GetOrCreateSellerAsync(
        UploadRecordData record,
        IDictionary<string, Seller> sellerCache,
        CancellationToken cancellationToken)
    {
        if (sellerCache.TryGetValue(record.SellerIdentification, out var cachedSeller))
        {
            return cachedSeller;
        }

        var localSeller = _dbContext.Sellers.Local.FirstOrDefault(entity =>
            entity.Identification.Equals(record.SellerIdentification, StringComparison.OrdinalIgnoreCase));
        if (localSeller is not null)
        {
            sellerCache[record.SellerIdentification] = localSeller;
            return localSeller;
        }

        var seller = await _dbContext.Sellers.FirstOrDefaultAsync(
            entity => entity.Identification == record.SellerIdentification,
            cancellationToken);

        if (seller is not null)
        {
            sellerCache[record.SellerIdentification] = seller;
            return seller;
        }

        seller = new Seller(
            Guid.NewGuid(),
            record.SellerIdentification,
            record.SellerName);

        await _dbContext.Sellers.AddAsync(seller, cancellationToken);
        sellerCache[record.SellerIdentification] = seller;
        return seller;
    }

    private async Task<Dictionary<string, Customer>> LoadCustomersAsync(IReadOnlyCollection<UploadRecordData> records, CancellationToken cancellationToken)
    {
        var identifiers = records.Select(record => record.CustomerIdentification)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var customers = await _dbContext.Customers
            .Where(entity => identifiers.Contains(entity.Identification))
            .ToListAsync(cancellationToken);

        return customers.ToDictionary(entity => entity.Identification, entity => entity, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, Product>> LoadProductsAsync(IReadOnlyCollection<UploadRecordData> records, CancellationToken cancellationToken)
    {
        var references = records.Select(record => record.ProductReference)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var products = await _dbContext.Products
            .Where(entity => references.Contains(entity.Reference))
            .ToListAsync(cancellationToken);

        return products.ToDictionary(entity => entity.Reference, entity => entity, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, Supplier>> LoadSuppliersAsync(IReadOnlyCollection<UploadRecordData> records, CancellationToken cancellationToken)
    {
        var identifiers = records.Select(record => record.SupplierIdentification)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var suppliers = await _dbContext.Suppliers
            .Where(entity => identifiers.Contains(entity.Identification))
            .ToListAsync(cancellationToken);

        return suppliers.ToDictionary(entity => entity.Identification, entity => entity, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, Seller>> LoadSellersAsync(IReadOnlyCollection<UploadRecordData> records, CancellationToken cancellationToken)
    {
        var identifiers = records.Select(record => record.SellerIdentification)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var sellers = await _dbContext.Sellers
            .Where(entity => identifiers.Contains(entity.Identification))
            .ToListAsync(cancellationToken);

        return sellers.ToDictionary(entity => entity.Identification, entity => entity, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<HashSet<string>> LoadExistingSalesAsync(IReadOnlyCollection<UploadRecordData> records, CancellationToken cancellationToken)
    {
        if (records.Count == 0)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var invoiceNumbers = records.Select(record => record.InvoiceNumber)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var minDate = records.Min(record => record.SaleDate).Date;
        var maxDate = records.Max(record => record.SaleDate).Date;

        var existing = await _dbContext.Sales.AsNoTracking()
            .Where(sale => invoiceNumbers.Contains(sale.InvoiceNumber) && sale.SaleDate >= minDate && sale.SaleDate <= maxDate)
            .Select(sale => new
            {
                sale.InvoiceNumber,
                sale.ProductId,
                sale.SaleDate,
                sale.SaleAmount
            })
            .ToListAsync(cancellationToken);

        var productReferenceById = await _dbContext.Products.AsNoTracking()
            .Where(product => existing.Select(item => item.ProductId).Distinct().Contains(product.Id))
            .ToDictionaryAsync(product => product.Id, product => product.Reference, cancellationToken);

        return existing
            .Where(item => productReferenceById.ContainsKey(item.ProductId))
            .Select(item => string.Join('|',
                item.InvoiceNumber,
                productReferenceById[item.ProductId],
                item.SaleDate.ToString(DateFormats.HtmlDate),
                item.SaleAmount.ToString(CultureInfo.InvariantCulture)))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static string BuildDuplicateKey(UploadRecordData record)
    {
        return string.Join('|',
            record.InvoiceNumber,
            record.ProductReference,
            record.SaleDate.ToString(DateFormats.HtmlDate),
            record.SaleAmount.ToString(System.Globalization.CultureInfo.InvariantCulture));
    }
}
