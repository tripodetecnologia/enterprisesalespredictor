using EnterpriseSalesPredictor.Application.DTOs.Reports;
using EnterpriseSalesPredictor.Application.Interfaces.Reports;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Reports;

public sealed class ReportReadService : IReportReadService
{
    private readonly AppDbContext _dbContext;

    public ReportReadService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReportDto> GetManagementReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);
        var totalSales = await sales.SumAsync(sale => (decimal?)sale.SaleAmount, cancellationToken) ?? 0m;
        var totalTransactions = await sales.CountAsync(cancellationToken);
        var averageTicket = totalTransactions == 0 ? 0m : totalSales / totalTransactions;
        var activeCustomers = await sales.Select(sale => sale.CustomerId).Distinct().CountAsync(cancellationToken);

        return BuildReport(
            "Management Report",
            CreateSection("Executive Summary",
                Metric("Total sales", totalSales),
                Metric("Transactions", totalTransactions),
                Metric("Average ticket", averageTicket),
                Metric("Active customers", activeCustomers)));
    }

    public async Task<ReportDto> GetCommercialReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var sales = ApplyDateFilters(_dbContext.Sales.AsNoTracking(), criteria);

        var topSeller = await sales
            .Join(_dbContext.Sellers.AsNoTracking(), sale => sale.SellerId, seller => seller.Id, (sale, seller) => new { sale, seller })
            .GroupBy(item => item.seller.Name)
            .Select(group => new { Name = group.Key, Amount = group.Sum(item => item.sale.SaleAmount) })
            .OrderByDescending(item => item.Amount)
            .FirstOrDefaultAsync(cancellationToken);

        var topProduct = await sales
            .Join(_dbContext.Products.AsNoTracking(), sale => sale.ProductId, product => product.Id, (sale, product) => new { sale, product })
            .GroupBy(item => item.product.Name)
            .Select(group => new { Name = group.Key, Amount = group.Sum(item => item.sale.SaleAmount) })
            .OrderByDescending(item => item.Amount)
            .FirstOrDefaultAsync(cancellationToken);

        var topCustomer = await sales
            .Join(_dbContext.Customers.AsNoTracking(), sale => sale.CustomerId, customer => customer.Id, (sale, customer) => new { sale, customer })
            .GroupBy(item => item.customer.Name)
            .Select(group => new { Name = group.Key, Amount = group.Sum(item => item.sale.SaleAmount) })
            .OrderByDescending(item => item.Amount)
            .FirstOrDefaultAsync(cancellationToken);

        return BuildReport(
            "Commercial Report",
            CreateSection("Top Commercial Drivers",
                Metric("Top seller", topSeller?.Name ?? "N/A"),
                Metric("Seller sales", topSeller?.Amount ?? 0m),
                Metric("Top product", topProduct?.Name ?? "N/A"),
                Metric("Product sales", topProduct?.Amount ?? 0m),
                Metric("Top customer", topCustomer?.Name ?? "N/A"),
                Metric("Customer sales", topCustomer?.Amount ?? 0m)));
    }

    public async Task<ReportDto> GetOperationalReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var uploads = await _dbContext.UploadedFiles.AsNoTracking()
            .OrderByDescending(upload => upload.UploadedAtUtc)
            .ToListAsync(cancellationToken);

        var auditCount = await _dbContext.AuditLogs.AsNoTracking().CountAsync(cancellationToken);
        var failedOrWarningUploads = uploads.Count(upload => upload.Status != UploadProcessStatus.Completed);

        return BuildReport(
            "Operational Report",
            CreateSection("Operational Health",
                Metric("Registered uploads", uploads.Count),
                Metric("Uploads with issues", failedOrWarningUploads),
                Metric("Audit events", auditCount),
                Metric("Last upload status", uploads.FirstOrDefault()?.Status.ToString() ?? "N/A")));
    }

    public async Task<ReportDto> GetReplenishmentReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var recommendations = _dbContext.ReplenishmentRecommendations.AsNoTracking();
        var total = await recommendations.CountAsync(cancellationToken);
        var pending = await recommendations.CountAsync(item => item.Status == RecommendationStatus.Pending, cancellationToken);
        var approved = await recommendations.CountAsync(item => item.Status == RecommendationStatus.Approved, cancellationToken);
        var rejected = await recommendations.CountAsync(item => item.Status == RecommendationStatus.Rejected, cancellationToken);

        return BuildReport(
            "Replenishment Report",
            CreateSection("Recommendation Status",
                Metric("Total recommendations", total),
                Metric("Pending", pending),
                Metric("Approved", approved),
                Metric("Rejected", rejected)));
    }

    public async Task<ReportDto> GetPredictiveReportAsync(ReportQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var forecasts = _dbContext.Forecasts.AsNoTracking();

        if (criteria.FromDate.HasValue)
        {
            forecasts = forecasts.Where(item => item.FromDate >= criteria.FromDate.Value);
        }

        if (criteria.ToDate.HasValue)
        {
            forecasts = forecasts.Where(item => item.ToDate <= criteria.ToDate.Value);
        }

        var totalForecasts = await forecasts.CountAsync(cancellationToken);
        var averageConfidence = totalForecasts == 0 ? 0m : await forecasts.AverageAsync(item => item.Confidence, cancellationToken);
        var projectedSales = await forecasts.SumAsync(item => (decimal?)item.ProjectedSales, cancellationToken) ?? 0m;

        return BuildReport(
            "Predictive Report",
            CreateSection("Forecast Summary",
                Metric("Registered forecasts", totalForecasts),
                Metric("Projected sales", projectedSales),
                Metric("Average confidence", averageConfidence)));
    }

    private static IQueryable<Sale> ApplyDateFilters(IQueryable<Sale> query, ReportQueryCriteria criteria)
    {
        if (criteria.FromDate.HasValue)
        {
            query = query.Where(sale => sale.SaleDate >= criteria.FromDate.Value);
        }

        if (criteria.ToDate.HasValue)
        {
            query = query.Where(sale => sale.SaleDate <= criteria.ToDate.Value);
        }

        return query;
    }

    private static ReportDto BuildReport(string title, params ReportSectionDto[] sections)
    {
        return new ReportDto
        {
            Title = title,
            GeneratedAtUtc = DateTime.UtcNow,
            Sections = sections
        };
    }

    private static ReportSectionDto CreateSection(string title, params ReportMetricDto[] metrics)
    {
        return new ReportSectionDto
        {
            Title = title,
            Metrics = metrics
        };
    }

    private static ReportMetricDto Metric(string label, string value)
    {
        return new ReportMetricDto
        {
            Label = label,
            Value = value
        };
    }

    private static ReportMetricDto Metric(string label, int value)
    {
        return Metric(label, value.ToString());
    }

    private static ReportMetricDto Metric(string label, decimal value)
    {
        return Metric(label, value.ToString("N2"));
    }
}
