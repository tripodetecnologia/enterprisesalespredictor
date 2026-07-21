using EnterpriseSalesPredictor.Application.DTOs.Sales;
using EnterpriseSalesPredictor.Application.Interfaces.Sales;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Sales;

public sealed class SalesReadService : ISalesReadService
{
    private readonly AppDbContext _dbContext;

    public SalesReadService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedSalesResult> QuerySalesAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(_dbContext.Sales.AsNoTracking(), criteria);
        query = ApplyLocationFilters(query, criteria);
        query = ApplySorting(query, criteria);

        var pageNumber = NormalizePageNumber(criteria.PageNumber);
        var pageSize = NormalizePageSize(criteria.PageSize);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Join(_dbContext.Customers.AsNoTracking(), sale => sale.CustomerId, customer => customer.Id, (sale, customer) => new { sale, customer })
            .Join(_dbContext.Products.AsNoTracking(), item => item.sale.ProductId, product => product.Id, (item, product) => new { item.sale, item.customer, product })
            .Join(_dbContext.Suppliers.AsNoTracking(), item => item.sale.SupplierId, supplier => supplier.Id, (item, supplier) => new { item.sale, item.customer, item.product, supplier })
            .Join(_dbContext.Sellers.AsNoTracking(), item => item.sale.SellerId, seller => seller.Id, (item, seller) => new { item.sale, item.customer, item.product, item.supplier, seller })
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new SaleDto
            {
                Id = item.sale.Id,
                InvoiceNumber = item.sale.InvoiceNumber,
                CustomerId = item.sale.CustomerId,
                CustomerName = item.customer.Name,
                ProductId = item.sale.ProductId,
                ProductName = item.product.Name,
                SupplierId = item.sale.SupplierId,
                SupplierName = item.supplier.Name,
                SellerId = item.sale.SellerId,
                SellerName = item.seller.Name,
                SaleDate = item.sale.SaleDate,
                Quantity = item.sale.Quantity,
                SaleAmount = item.sale.SaleAmount,
                PaymentMethod = item.sale.PaymentMethod
            })
            .ToArrayAsync(cancellationToken);

        return new PagedSalesResult
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesByCustomerAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyFilters(_dbContext.Sales.AsNoTracking(), criteria);
        sales = ApplyLocationFilters(sales, criteria);

        return await sales
            .Join(
                _dbContext.Customers.AsNoTracking(),
                sale => sale.CustomerId,
                customer => customer.Id,
                (sale, customer) => new { sale, customer })
            .GroupBy(item => item.customer.Name)
            .Select(group => new SalesDimensionSummaryDto
            {
                DimensionKey = group.Key,
                TotalAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                Transactions = group.Count()
            })
            .OrderByDescending(item => item.TotalAmount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesByProductAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyFilters(_dbContext.Sales.AsNoTracking(), criteria);
        sales = ApplyLocationFilters(sales, criteria);

        return await sales
            .Join(
                _dbContext.Products.AsNoTracking(),
                sale => sale.ProductId,
                product => product.Id,
                (sale, product) => new { sale, product })
            .GroupBy(item => item.product.Name)
            .Select(group => new SalesDimensionSummaryDto
            {
                DimensionKey = group.Key,
                TotalAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                Transactions = group.Count()
            })
            .OrderByDescending(item => item.TotalAmount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesBySupplierAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyFilters(_dbContext.Sales.AsNoTracking(), criteria);
        sales = ApplyLocationFilters(sales, criteria);

        return await sales
            .Join(
                _dbContext.Suppliers.AsNoTracking(),
                sale => sale.SupplierId,
                supplier => supplier.Id,
                (sale, supplier) => new { sale, supplier })
            .GroupBy(item => item.supplier.Name)
            .Select(group => new SalesDimensionSummaryDto
            {
                DimensionKey = group.Key,
                TotalAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                Transactions = group.Count()
            })
            .OrderByDescending(item => item.TotalAmount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesBySellerAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyFilters(_dbContext.Sales.AsNoTracking(), criteria);
        sales = ApplyLocationFilters(sales, criteria);

        return await sales
            .Join(
                _dbContext.Sellers.AsNoTracking(),
                sale => sale.SellerId,
                seller => seller.Id,
                (sale, seller) => new { sale, seller })
            .GroupBy(item => item.seller.Name)
            .Select(group => new SalesDimensionSummaryDto
            {
                DimensionKey = group.Key,
                TotalAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                Transactions = group.Count()
            })
            .OrderByDescending(item => item.TotalAmount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SalesDimensionSummaryDto>> GetSalesByLocationAsync(SalesQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyFilters(_dbContext.Sales.AsNoTracking(), criteria);
        sales = ApplyLocationFilters(sales, criteria);

        return await sales
            .Join(
                _dbContext.Customers.AsNoTracking(),
                sale => sale.CustomerId,
                customer => customer.Id,
                (sale, customer) => new { sale, customer })
            .GroupBy(item => string.Join(" / ", item.customer.City, item.customer.Zone))
            .Select(group => new SalesDimensionSummaryDto
            {
                DimensionKey = group.Key,
                TotalAmount = group.Sum(item => item.sale.SaleAmount),
                TotalQuantity = group.Sum(item => item.sale.Quantity),
                Transactions = group.Count()
            })
            .OrderByDescending(item => item.TotalAmount)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SalesComparisonSummaryDto>> GetSalesComparisonsAsync(
        string period,
        SalesQueryCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var normalizedPeriod = NormalizePeriod(period);
        var sales = ApplyFilters(_dbContext.Sales.AsNoTracking(), criteria);
        sales = ApplyLocationFilters(sales, criteria);

        return normalizedPeriod switch
        {
            "year" => await sales
                .GroupBy(sale => sale.SaleDate.Year)
                .Select(group => BuildComparisonSummary("year", group.Key.ToString(), group))
                .OrderBy(item => item.PeriodKey)
                .ToArrayAsync(cancellationToken),

            "semester" => await sales
                .GroupBy(sale => new { sale.SaleDate.Year, Semester = sale.SaleDate.Month <= 6 ? 1 : 2 })
                .Select(group => BuildComparisonSummary("semester", $"{group.Key.Year}-S{group.Key.Semester}", group))
                .OrderBy(item => item.PeriodKey)
                .ToArrayAsync(cancellationToken),

            "quarter" => await sales
                .GroupBy(sale => new { sale.SaleDate.Year, Quarter = ((sale.SaleDate.Month - 1) / 3) + 1 })
                .Select(group => BuildComparisonSummary("quarter", $"{group.Key.Year}-Q{group.Key.Quarter}", group))
                .OrderBy(item => item.PeriodKey)
                .ToArrayAsync(cancellationToken),

            _ => await sales
                .GroupBy(sale => new { sale.SaleDate.Year, sale.SaleDate.Month })
                .Select(group => BuildComparisonSummary("month", $"{group.Key.Year}-{group.Key.Month:00}", group))
                .OrderBy(item => item.PeriodKey)
                .ToArrayAsync(cancellationToken)
        };
    }

    private static IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, SalesQueryCriteria criteria)
    {
        if (criteria.FromDate.HasValue)
        {
            query = query.Where(sale => sale.SaleDate >= criteria.FromDate.Value);
        }

        if (criteria.ToDate.HasValue)
        {
            query = query.Where(sale => sale.SaleDate <= criteria.ToDate.Value);
        }

        if (criteria.CustomerId.HasValue)
        {
            query = query.Where(sale => sale.CustomerId == criteria.CustomerId.Value);
        }

        if (criteria.ProductId.HasValue)
        {
            query = query.Where(sale => sale.ProductId == criteria.ProductId.Value);
        }

        if (criteria.SupplierId.HasValue)
        {
            query = query.Where(sale => sale.SupplierId == criteria.SupplierId.Value);
        }

        if (criteria.SellerId.HasValue)
        {
            query = query.Where(sale => sale.SellerId == criteria.SellerId.Value);
        }

        return query;
    }

    private IQueryable<Sale> ApplyLocationFilters(IQueryable<Sale> query, SalesQueryCriteria criteria)
    {
        if (!string.IsNullOrWhiteSpace(criteria.City))
        {
            var city = criteria.City.Trim();
            query = query.Where(sale => _dbContext.Customers
                .Any(customer => customer.Id == sale.CustomerId && customer.City.Contains(city)));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Zone))
        {
            var zone = criteria.Zone.Trim();
            query = query.Where(sale => _dbContext.Customers
                .Any(customer => customer.Id == sale.CustomerId && customer.Zone.Contains(zone)));
        }

        return query;
    }

    private IQueryable<Sale> ApplySorting(IQueryable<Sale> query, SalesQueryCriteria criteria)
    {
        var sortBy = criteria.SortBy?.Trim().ToLowerInvariant() ?? "saledate";
        var ascending = string.Equals(criteria.SortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        return sortBy switch
        {
            "saleamount" => ascending ? query.OrderBy(sale => sale.SaleAmount) : query.OrderByDescending(sale => sale.SaleAmount),
            "quantity" => ascending ? query.OrderBy(sale => sale.Quantity) : query.OrderByDescending(sale => sale.Quantity),
            "invoicenumber" => ascending ? query.OrderBy(sale => sale.InvoiceNumber) : query.OrderByDescending(sale => sale.InvoiceNumber),
            _ => ascending ? query.OrderBy(sale => sale.SaleDate) : query.OrderByDescending(sale => sale.SaleDate)
        };
    }

    private static SalesComparisonSummaryDto BuildComparisonSummary<TKey>(string periodType, string periodKey, IGrouping<TKey, Sale> group)
    {
        return new SalesComparisonSummaryDto
        {
            PeriodType = periodType,
            PeriodKey = periodKey,
            TotalAmount = group.Sum(item => item.SaleAmount),
            TotalQuantity = group.Sum(item => item.Quantity),
            Transactions = group.Count()
        };
    }

    private static int NormalizePageNumber(int pageNumber)
    {
        return pageNumber <= 0 ? 1 : pageNumber;
    }

    private static int NormalizePageSize(int pageSize)
    {
        if (pageSize <= 0)
        {
            return 20;
        }

        return Math.Min(pageSize, 5000);
    }

    private static string NormalizePeriod(string period)
    {
        return period.Trim().ToLowerInvariant() switch
        {
            "year" => "year",
            "semester" => "semester",
            "quarter" => "quarter",
            _ => "month"
        };
    }
}
