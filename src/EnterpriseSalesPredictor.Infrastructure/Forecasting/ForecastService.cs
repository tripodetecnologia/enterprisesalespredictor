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
        if (!ForecastRules.IsValidForecastRange(query.FromDate, query.ToDate))
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(query.ToDate), $"Forecast horizon must be between {ForecastRules.MinimumForecastDays} and {ForecastRules.MaximumForecastDays} days.")
            });
        }

        var historicalSales = _dbContext.Sales.AsNoTracking().AsQueryable();

        if (query.ProductId.HasValue)
        {
            historicalSales = historicalSales.Where(sale => sale.ProductId == query.ProductId.Value);
        }

        if (query.CustomerId.HasValue)
        {
            historicalSales = historicalSales.Where(sale => sale.CustomerId == query.CustomerId.Value);
        }

        var horizonDays = (query.ToDate.Date - query.FromDate.Date).Days + 1;
        var lookbackDays = Math.Max(horizonDays * 3, 30);
        var lookbackStart = query.FromDate.Date.AddDays(-lookbackDays);
        historicalSales = historicalSales.Where(sale => sale.SaleDate.Date >= lookbackStart && sale.SaleDate.Date < query.FromDate.Date);

        var totalHistoricalSales = await historicalSales.SumAsync(sale => (decimal?)sale.SaleAmount, cancellationToken) ?? 0m;
        var distinctSaleDays = await historicalSales
            .Select(sale => sale.SaleDate.Date)
            .Distinct()
            .CountAsync(cancellationToken);

        var effectiveDays = Math.Max(distinctSaleDays, 1);
        var averageDailySales = totalHistoricalSales / effectiveDays;
        var projectedSales = decimal.Round(averageDailySales * horizonDays, 2, MidpointRounding.AwayFromZero);

        var confidence = distinctSaleDays switch
        {
            >= 60 => 0.90m,
            >= 30 => 0.80m,
            >= 14 => 0.70m,
            > 0 => 0.55m,
            _ => 0.35m
        };

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

        var explanation = $"Projection uses average daily sales from the previous {lookbackDays} days ({distinctSaleDays} active sale days) and extends it across a {horizonDays}-day horizon.";

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = query.RequestedBy,
            Action = "ForecastGenerated",
            Module = "Forecasting",
            Details = $"ForecastId={forecast.Id}; FromDate={forecast.FromDate:yyyy-MM-dd}; ToDate={forecast.ToDate:yyyy-MM-dd}; ProductId={query.ProductId}; CustomerId={query.CustomerId}; Confidence={confidence}; ProjectedSales={projectedSales}"
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
            Explanation = explanation
        };
    }
}
