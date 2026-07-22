using EnterpriseSalesPredictor.Application.DTOs.Forecasting;
using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Forecasting;
using EnterpriseSalesPredictor.Application.Validators;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Domain.Rules;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Forecasting;

public sealed class ForecastService : IForecastService
{
    private readonly AppDbContext _dbContext;
    private readonly IAuditLogService _auditLogService;

    public ForecastService(AppDbContext dbContext, IAuditLogService auditLogService)
    {
        _dbContext = dbContext;
        _auditLogService = auditLogService;
    }

    public async Task<ForecastDto> GenerateForecastAsync(ForecastQuery query, CancellationToken cancellationToken = default)
    {
        if (query.FromDate == default)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(query.FromDate), "La fecha de inicio es obligatoria.")
            });
        }

        if (query.ToDate == default)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(query.ToDate), "La fecha de fin es obligatoria.")
            });
        }

        if (!ForecastRules.IsValidForecastRange(query.FromDate, query.ToDate))
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(query.ToDate), $"El horizonte de proyección debe estar entre {ForecastRules.MinimumForecastDays} y {ForecastRules.MaximumForecastDays} días.")
            });
        }

        var horizonDays = (query.ToDate.Date - query.FromDate.Date).Days + 1;
        var lookbackDays = Math.Max(horizonDays * 3, 90);
        var lookbackStart = query.FromDate.Date.AddDays(-lookbackDays);
        var historicalSales = _dbContext.Sales.AsNoTracking()
            .Where(sale => sale.SaleDate.Date >= lookbackStart && sale.SaleDate.Date < query.FromDate.Date);

        if (!string.IsNullOrWhiteSpace(query.ProductName))
        {
            var productName = query.ProductName.Trim();
            var productIds = await _dbContext.Products.AsNoTracking()
                .Where(item => item.Name == productName)
                .Select(item => item.Id)
                .ToArrayAsync(cancellationToken);

            historicalSales = historicalSales.Where(sale => productIds.Contains(sale.ProductId));
        }

        var customerForecasts = await BuildCustomerMonthlyForecastsAsync(historicalSales, query, cancellationToken);
        var productForecasts = await BuildProductMonthlyForecastsAsync(historicalSales, query, cancellationToken);

        var projectedSales = decimal.Round(
            Math.Max(customerForecasts.Sum(item => item.ProjectedSales), productForecasts.Sum(item => item.ProjectedSales)),
            2,
            MidpointRounding.AwayFromZero);

        var allConfidences = customerForecasts.Select(item => item.Confidence)
            .Concat(productForecasts.Select(item => item.Confidence))
            .ToArray();

        var confidence = allConfidences.Length == 0
            ? 0.35m
            : decimal.Round(allConfidences.Average(), 2, MidpointRounding.AwayFromZero);

        var forecast = new Forecast(
            Guid.NewGuid(),
            DateTime.UtcNow,
            query.FromDate.Date,
            query.ToDate.Date,
            projectedSales,
            confidence,
            query.RequestedBy);

        await _dbContext.Forecasts.AddAsync(forecast, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var explanation = $"La proyección distribuye el rango seleccionado por mes y usa promedios diarios calculados sobre los {lookbackDays} días previos para estimar ventas por cliente y ventas/unidades por producto.";

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = query.RequestedBy,
            Action = "ForecastGenerated",
            Module = "Forecasting",
            Details = $"ForecastId={forecast.Id}; FromDate={forecast.FromDate:yyyy-MM-dd}; ToDate={forecast.ToDate:yyyy-MM-dd}; Confidence={confidence}; ProjectedSales={projectedSales}; CustomerCards={customerForecasts.Count}; ProductCards={productForecasts.Count}"
        }, cancellationToken);

        return new ForecastDto
        {
            Id = forecast.Id,
            GeneratedAtUtc = forecast.GeneratedAtUtc,
            FromDate = forecast.FromDate,
            ToDate = forecast.ToDate,
            ProjectedSales = forecast.ProjectedSales,
            Confidence = forecast.Confidence,
            GeneratedBy = forecast.GeneratedBy,
            Explanation = explanation,
            CustomerMonthlyForecasts = customerForecasts,
            ProductMonthlyForecasts = productForecasts
        };
    }

    public async Task<ForecastOptionsDto> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _dbContext.Customers.AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new ForecastLookupDto
            {
                Id = item.Id,
                Name = item.Name
            })
            .ToArrayAsync(cancellationToken);

        var productRows = await _dbContext.Products.AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new ForecastLookupDto
            {
                Id = item.Id,
                Name = item.Name
            })
            .ToListAsync(cancellationToken);

        var products = productRows
            .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Name)
            .ToArray();

        return new ForecastOptionsDto
        {
            Customers = customers,
            Products = products
        };
    }

    private async Task<IReadOnlyCollection<CustomerMonthlyForecastDto>> BuildCustomerMonthlyForecastsAsync(
        IQueryable<Sale> historicalSales,
        ForecastQuery query,
        CancellationToken cancellationToken)
    {
        var customerBase = await historicalSales
            .Join(_dbContext.Customers.AsNoTracking(), sale => sale.CustomerId, customer => customer.Id, (sale, customer) => new { sale, customer })
            .GroupBy(item => new { item.customer.Id, item.customer.Name })
            .Select(group => new
            {
                group.Key.Id,
                group.Key.Name,
                TotalSales = group.Sum(item => item.sale.SaleAmount),
                ActiveDays = group.Select(item => item.sale.SaleDate.Date).Distinct().Count()
            })
            .OrderByDescending(item => item.TotalSales)
            .Take(6)
            .ToListAsync(cancellationToken);

        var result = new List<CustomerMonthlyForecastDto>();
        foreach (var month in BuildMonthSlices(query.FromDate, query.ToDate))
        {
            foreach (var customer in customerBase)
            {
                var avgDailySales = customer.ActiveDays == 0 ? 0m : customer.TotalSales / customer.ActiveDays;
                result.Add(new CustomerMonthlyForecastDto
                {
                    MonthLabel = month.Label,
                    CustomerId = customer.Id,
                    CustomerName = customer.Name,
                    ProjectedSales = decimal.Round(avgDailySales * month.Days, 2, MidpointRounding.AwayFromZero),
                    Confidence = CalculateConfidence(customer.ActiveDays)
                });
            }
        }

        return result;
    }

    private async Task<IReadOnlyCollection<ProductMonthlyForecastDto>> BuildProductMonthlyForecastsAsync(
        IQueryable<Sale> historicalSales,
        ForecastQuery query,
        CancellationToken cancellationToken)
    {
        var productBase = await historicalSales
            .Join(_dbContext.Products.AsNoTracking(), sale => sale.ProductId, product => product.Id, (sale, product) => new { sale, product })
            .GroupBy(item => item.product.Name)
            .Select(group => new
            {
                Name = group.Key,
                TotalSales = group.Sum(item => item.sale.SaleAmount),
                TotalUnits = group.Sum(item => item.sale.Quantity),
                ActiveDays = group.Select(item => item.sale.SaleDate.Date).Distinct().Count()
            })
            .OrderByDescending(item => item.TotalSales)
            .Take(6)
            .ToListAsync(cancellationToken);

        var result = new List<ProductMonthlyForecastDto>();
        foreach (var month in BuildMonthSlices(query.FromDate, query.ToDate))
        {
            foreach (var product in productBase)
            {
                var avgDailySales = product.ActiveDays == 0 ? 0m : product.TotalSales / product.ActiveDays;
                var avgDailyUnits = product.ActiveDays == 0 ? 0m : product.TotalUnits / product.ActiveDays;
                result.Add(new ProductMonthlyForecastDto
                {
                    MonthLabel = month.Label,
                    ProductId = Guid.Empty,
                    ProductName = product.Name,
                    ProjectedSales = decimal.Round(avgDailySales * month.Days, 2, MidpointRounding.AwayFromZero),
                    ProjectedUnits = decimal.Round(avgDailyUnits * month.Days, 2, MidpointRounding.AwayFromZero),
                    Confidence = CalculateConfidence(product.ActiveDays)
                });
            }
        }

        return result;
    }

    private static IReadOnlyCollection<(string Label, int Days)> BuildMonthSlices(DateTime fromDate, DateTime toDate)
    {
        var slices = new List<(string Label, int Days)>();
        var cursor = new DateTime(fromDate.Year, fromDate.Month, 1);
        var endMonth = new DateTime(toDate.Year, toDate.Month, 1);

        while (cursor <= endMonth)
        {
            var sliceStart = cursor < fromDate.Date ? fromDate.Date : cursor;
            var monthEnd = new DateTime(cursor.Year, cursor.Month, DateTime.DaysInMonth(cursor.Year, cursor.Month));
            var sliceEnd = monthEnd > toDate.Date ? toDate.Date : monthEnd;
            var days = (sliceEnd - sliceStart).Days + 1;

            if (days > 0)
            {
                slices.Add(($"{cursor:yyyy-MM}", days));
            }

            cursor = cursor.AddMonths(1);
        }

        return slices;
    }

    private static decimal CalculateConfidence(int activeDays)
    {
        return activeDays switch
        {
            >= 60 => 0.90m,
            >= 30 => 0.80m,
            >= 14 => 0.70m,
            > 0 => 0.55m,
            _ => 0.35m
        };
    }
}
