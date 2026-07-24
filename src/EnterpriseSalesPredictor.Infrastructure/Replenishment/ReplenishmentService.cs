using EnterpriseSalesPredictor.Application.Constants;
using EnterpriseSalesPredictor.Application.DTOs.Replenishment;
using EnterpriseSalesPredictor.Application.Interfaces;
using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Replenishment;
using EnterpriseSalesPredictor.Application.Validators;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Domain.Rules;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Replenishment;

public sealed class ReplenishmentService : IReplenishmentService
{
    private readonly AppDbContext _dbContext;
    private readonly IAuditLogService _auditLogService;
    private readonly IUnitOfWork _unitOfWork;

    public ReplenishmentService(AppDbContext dbContext, IAuditLogService auditLogService, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _auditLogService = auditLogService;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedReplenishmentProjectionResultDto> GetProjectionsAsync(ReplenishmentProjectionQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        ValidateProjectionRange(criteria.FromDate, criteria.ToDate);

        var productsQuery = _dbContext.Products.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(criteria.ProductName))
        {
            var productName = criteria.ProductName.Trim();
            productsQuery = productsQuery.Where(item => item.Name == productName);
        }

        var products = await productsQuery.OrderBy(item => item.Name).ToListAsync(cancellationToken);
        var months = BuildMonthSlices(criteria.FromDate!.Value, criteria.ToDate!.Value);
        var projections = new List<ReplenishmentProjectionDto>();

        foreach (var month in months)
        {
            foreach (var product in products)
            {
                var projection = await BuildProjectionAsync(product, month, criteria.CustomerId, cancellationToken);
                if (projection is not null)
                {
                    projections.Add(projection);
                }
            }
        }

        var ordered = projections
            .OrderBy(item => item.ProjectionMonth)
            .ThenBy(item => item.ProductName)
            .ToArray();

        var pageNumber = Math.Max(criteria.PageNumber, 1);
        var pageSize = ReplenishmentPolicy.DefaultPageSize;
        var totalCount = ordered.Length;

        return new PagedReplenishmentProjectionResultDto
        {
            Items = ordered.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArray(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    public async Task<ReplenishmentRecommendationDto> SubmitProjectionAsync(SubmitReplenishmentProjectionCommand command, CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(item => item.Id == command.ProductId, cancellationToken);
        if (product is null)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.ProductId), ReplenishmentMessages.ProductNotFound)
            });
        }

        if (command.RecommendedUnits <= 0m)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.RecommendedUnits), ReplenishmentMessages.RecommendedUnitsMustBeGreaterThanZero)
            });
        }

        var projectionMonth = new DateTime(command.ProjectionMonth.Year, command.ProjectionMonth.Month, 1);
        var existing = await _dbContext.ReplenishmentRecommendations
            .Include(item => item.Product)
            .FirstOrDefaultAsync(item => item.ProductId == command.ProductId && item.RecommendedForMonth == projectionMonth && item.Status == RecommendationStatus.Pending, cancellationToken);

        var rationale = $"Sugerencia enviada a aprobación para {projectionMonth.ToString(DateFormats.MonthKey)}. Stock actual: {command.CurrentStockUnits}. Cantidad recomendada: {command.RecommendedUnits:N0}.";
        var confidence = command.CurrentStockUnits <= ReplenishmentPolicy.MinimumStockUnits
            ? ReplenishmentPolicy.StockoutRiskConfidence
            : ReplenishmentPolicy.StandardConfidence;

        if (existing is not null)
        {
            existing.Refresh(command.RecommendedUnits, confidence, rationale);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await RegisterSubmittedAuditAsync(command.RequestedBy, existing, cancellationToken);
            return Map(existing, product);
        }

        var recommendation = new ReplenishmentRecommendation(
            Guid.NewGuid(),
            command.ProductId,
            DateTime.UtcNow,
            projectionMonth,
            command.RecommendedUnits,
            confidence,
            rationale);

        await _dbContext.ReplenishmentRecommendations.AddAsync(recommendation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await RegisterSubmittedAuditAsync(command.RequestedBy, recommendation, cancellationToken);
        return Map(recommendation, product);
    }

    public async Task<IReadOnlyCollection<ReplenishmentRecommendationDto>> GenerateRecommendationAsync(GenerateReplenishmentCommand command, CancellationToken cancellationToken = default)
    {
        ValidateProjectionRange(command.FromDate, command.ToDate);

        var products = await _dbContext.Products.AsNoTracking().OrderBy(item => item.Name).ToListAsync(cancellationToken);
        var months = BuildMonthSlices(command.FromDate, command.ToDate);
        var recommendations = new List<ReplenishmentRecommendationDto>();

        foreach (var month in months)
        {
            foreach (var product in products)
            {
                var generated = await GenerateForProductAndMonthAsync(product, month, command.RequestedBy, cancellationToken);
                if (generated is not null)
                {
                    recommendations.Add(generated);
                }
            }
        }

        return recommendations
            .OrderBy(item => item.RecommendedForMonth)
            .ThenBy(item => item.ProductName)
            .ToArray();
    }

    public async Task<ReplenishmentRecommendationDto> ReviewRecommendationAsync(ReviewReplenishmentCommand command, CancellationToken cancellationToken = default)
    {
        var recommendation = await _dbContext.ReplenishmentRecommendations
            .Include(item => item.Product)
            .FirstOrDefaultAsync(item => item.Id == command.RecommendationId, cancellationToken);

        if (recommendation is null)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.RecommendationId), ReplenishmentMessages.RecommendationNotFound)
            });
        }

        if (!ReplenishmentRules.CanApprove(command.ReviewerRole))
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.ReviewerRole), ReplenishmentMessages.ReviewerRoleNotAllowed)
            });
        }

        var action = command.Action.Trim().ToLowerInvariant();

        if (action == RecommendationReviewActions.Approve)
        {
            recommendation.Approve(command.Reviewer, command.Notes);
        }
        else if (action == RecommendationReviewActions.Reject)
        {
            recommendation.Reject(command.Reviewer, command.Notes);
        }
        else if (action == RecommendationReviewActions.Analysis)
        {
            recommendation.MarkForAnalysis(command.Reviewer, command.Notes);
        }
        else
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.Action), ReplenishmentMessages.InvalidReviewAction)
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = command.Reviewer,
            Action = action == RecommendationReviewActions.Approve ? "RecommendationApproved" : action == RecommendationReviewActions.Reject ? "RecommendationRejected" : "RecommendationMarkedForAnalysis",
            Module = "Replenishment",
            Details = $"RecommendationId={recommendation.Id}; Notes={command.Notes}"
        }, cancellationToken);

        return Map(recommendation, recommendation.Product);
    }

    public async Task<PagedReplenishmentResultDto> GetRecommendationsAsync(ReplenishmentQueryCriteria criteria, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ReplenishmentRecommendations.AsNoTracking()
            .Include(item => item.Product)
            .AsQueryable();

        if (criteria.FromDate.HasValue)
        {
            var fromMonth = new DateTime(criteria.FromDate.Value.Year, criteria.FromDate.Value.Month, 1);
            query = query.Where(item => item.RecommendedForMonth >= fromMonth);
        }

        if (criteria.ToDate.HasValue)
        {
            var toMonth = new DateTime(criteria.ToDate.Value.Year, criteria.ToDate.Value.Month, 1);
            query = query.Where(item => item.RecommendedForMonth <= toMonth);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Status) && Enum.TryParse<RecommendationStatus>(criteria.Status, true, out var status))
        {
            query = query.Where(item => item.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var pageNumber = Math.Max(criteria.PageNumber, 1);
        var pageSize = ReplenishmentPolicy.DefaultPageSize;

        var items = await query
            .OrderBy(item => item.RecommendedForMonth)
            .ThenBy(item => item.Product!.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedReplenishmentResultDto
        {
            Items = items.Select(item => Map(item, item.Product)).ToArray(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    private async Task<ReplenishmentProjectionDto?> BuildProjectionAsync(Product product, (DateTime MonthStart, int Days) month, Guid? customerId, CancellationToken cancellationToken)
    {
        int daysLookBack = ReplenishmentPolicy.ProjectionLookbackDays;
        var lookbackStart = month.MonthStart.AddDays(-daysLookBack);
        var salesQuery = _dbContext.Sales.AsNoTracking()
            .Where(item => item.ProductId == product.Id && item.SaleDate >= lookbackStart && item.SaleDate < month.MonthStart);

        if (customerId.HasValue)
        {
            salesQuery = salesQuery.Where(item => item.CustomerId == customerId.Value);
        }

        var sales = await salesQuery.ToListAsync(cancellationToken);
        var distinctDays = sales.Select(item => item.SaleDate.Date).Distinct().Count();
        var totalQuantitySold = sales.Sum(item => item.Quantity);
        var averageDailyDemand = distinctDays == 0 ? 0m : totalQuantitySold / distinctDays;
        var projectedDemand = decimal.Round(averageDailyDemand * month.Days, 2, MidpointRounding.AwayFromZero);
        var availableUnits = product.AvailableUnits;
        var shortage = Math.Max(projectedDemand - availableUnits, 0m);
        var lowRotation = totalQuantitySold <= ReplenishmentPolicy.LowRotationUnits;
        var riskOfStockout = projectedDemand > 0m && availableUnits <= Math.Max(ReplenishmentPolicy.MinimumStockUnits, projectedDemand * ReplenishmentPolicy.StockoutRiskDemandRatio);

        if (!ReplenishmentRules.ShouldGenerateRecommendation(projectedDemand, availableUnits) && !riskOfStockout)
        {
            return null;
        }

        var recommendedUnits = decimal.Round(Math.Max(shortage, projectedDemand * ReplenishmentPolicy.RecommendedBufferRatio), 2, MidpointRounding.AwayFromZero);
        var confidence = lowRotation ? ReplenishmentPolicy.LowRotationConfidence : (riskOfStockout ? ReplenishmentPolicy.StockoutRiskConfidence : ReplenishmentPolicy.StandardConfidence);
        var rationale = $"Demanda proyectada {projectedDemand:N0}, stock actual {availableUnits}, sugerencia {recommendedUnits:N0}.";

        return new ReplenishmentProjectionDto
        {
            ProjectionMonth = month.MonthStart,
            ProductId = product.Id,
            ProductName = product.Name,
            ProductType = product.ProductType,
            ProductReference = product.Reference,
            ProductBrand = product.Brand,
            RecommendedUnits = recommendedUnits,
            CurrentStockUnits = availableUnits,
            Confidence = confidence,
            Rationale = rationale
        };
    }

    private void ValidateProjectionRange(DateTime? fromDate, DateTime? toDate)
    {
        if (!fromDate.HasValue || !toDate.HasValue)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(fromDate), ReplenishmentMessages.InvalidDateRange)
            });
        }

        var fromMonth = new DateTime(fromDate.Value.Year, fromDate.Value.Month, 1);
        var toMonth = new DateTime(toDate.Value.Year, toDate.Value.Month, 1);
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        if (fromMonth < currentMonth || toMonth < currentMonth)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(fromDate), ReplenishmentMessages.ProjectionDatesMustBeFuture)
            });
        }

        if (!ForecastRules.IsValidForecastRange(fromMonth, toMonth))
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(toDate), ReplenishmentMessages.ProjectionRangeOutOfBounds(ForecastRules.MinimumForecastDays, ForecastRules.MaximumForecastDays))
            });
        }
    }

    private async Task RegisterSubmittedAuditAsync(string requestedBy, ReplenishmentRecommendation recommendation, CancellationToken cancellationToken)
    {
        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = requestedBy,
            Action = "RecommendationSubmittedForApproval",
            Module = "Replenishment",
            Details = $"RecommendationId={recommendation.Id}; ProductId={recommendation.ProductId}; Month={recommendation.RecommendedForMonth.ToString(DateFormats.MonthKey)}; RecommendedUnits={recommendation.RecommendedUnits}"
        }, cancellationToken);
    }

    private async Task<ReplenishmentRecommendationDto?> GenerateForProductAndMonthAsync(
        Product product,
        (DateTime MonthStart, int Days) month,
        string requestedBy,
        CancellationToken cancellationToken)
    {
        var lookbackStart = month.MonthStart.AddDays(-ReplenishmentPolicy.RecommendationLookbackDays);
        var sales = await _dbContext.Sales.AsNoTracking()
            .Where(item => item.ProductId == product.Id && item.SaleDate >= lookbackStart && item.SaleDate < month.MonthStart)
            .ToListAsync(cancellationToken);

        var distinctDays = sales.Select(item => item.SaleDate.Date).Distinct().Count();
        var totalQuantitySold = sales.Sum(item => item.Quantity);
        var averageDailyDemand = distinctDays == 0 ? 0m : totalQuantitySold / distinctDays;
        var projectedDemand = decimal.Round(averageDailyDemand * month.Days, 2, MidpointRounding.AwayFromZero);
        var availableUnits = product.AvailableUnits;
        var shortage = Math.Max(projectedDemand - availableUnits, 0m);
        var lowRotation = totalQuantitySold <= ReplenishmentPolicy.LowRotationUnits;
        var riskOfStockout = projectedDemand > 0m && availableUnits <= Math.Max(ReplenishmentPolicy.MinimumStockUnits, projectedDemand * ReplenishmentPolicy.StockoutRiskDemandRatio);

        if (!ReplenishmentRules.ShouldGenerateRecommendation(projectedDemand, availableUnits) && !riskOfStockout)
        {
            return null;
        }

        var recommendedUnits = decimal.Round(Math.Max(shortage, projectedDemand * ReplenishmentPolicy.RecommendedBufferRatio), 2, MidpointRounding.AwayFromZero);
        var confidence = lowRotation ? ReplenishmentPolicy.LowRotationConfidence : (riskOfStockout ? ReplenishmentPolicy.StockoutRiskConfidence : ReplenishmentPolicy.StandardConfidence);

        var rationaleParts = new List<string>
        {
            $"La demanda proyectada para {month.MonthStart.ToString(DateFormats.MonthKey)} es {projectedDemand:N0} unidades basada en {distinctDays} días activos de venta.",
            $"El stock disponible actual es {availableUnits} unidades.",
            $"La necesidad estimada de compra es {recommendedUnits:N0} unidades."
        };

        if (riskOfStockout)
        {
            rationaleParts.Add($"Se detectó riesgo de agotamiento porque el stock disponible cubre menos del {ReplenishmentPolicy.StockoutRiskDemandRatio:P0} de la demanda proyectada.");
        }

        if (lowRotation)
        {
            rationaleParts.Add($"Se detectó baja rotación en los últimos {ReplenishmentPolicy.RecommendationLookbackDays} días, por lo que la confianza disminuye y la recomendación debe revisarse con cuidado.");
        }

        var monthMarker = new DateTime(month.MonthStart.Year, month.MonthStart.Month, 1);
        var existing = await _dbContext.ReplenishmentRecommendations
            .FirstOrDefaultAsync(item => item.ProductId == product.Id && item.RecommendedForMonth == monthMarker &&
                                         (item.Status == RecommendationStatus.Pending || item.Status == RecommendationStatus.LowConfidence), cancellationToken);

        if (existing is not null)
        {
            existing.Refresh(recommendedUnits, confidence, string.Join(' ', rationaleParts));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.RecordAsync(new CreateAuditLogCommand
            {
                Actor = requestedBy,
                Action = "RecommendationRefreshed",
                Module = "Replenishment",
                Details = $"RecommendationId={existing.Id}; ProductId={product.Id}; Month={monthMarker.ToString(DateFormats.MonthKey)}; RecommendedUnits={recommendedUnits}; Confidence={confidence}"
            }, cancellationToken);

            return Map(existing, product);
        }

        var recommendation = new ReplenishmentRecommendation(
            Guid.NewGuid(),
            product.Id,
            DateTime.UtcNow,
            monthMarker,
            recommendedUnits,
            confidence,
            string.Join(' ', rationaleParts));

        await _dbContext.ReplenishmentRecommendations.AddAsync(recommendation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = requestedBy,
            Action = "RecommendationGenerated",
            Module = "Replenishment",
            Details = $"RecommendationId={recommendation.Id}; ProductId={product.Id}; Month={monthMarker.ToString(DateFormats.MonthKey)}; ProjectedDemand={projectedDemand}; AvailableUnits={availableUnits}; RecommendedUnits={recommendedUnits}; Confidence={confidence}"
        }, cancellationToken);

        return Map(recommendation, product);
    }

    private static IReadOnlyCollection<(DateTime MonthStart, int Days)> BuildMonthSlices(DateTime fromDate, DateTime toDate)
    {
        var slices = new List<(DateTime MonthStart, int Days)>();
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
                slices.Add((cursor, days));
            }

            cursor = cursor.AddMonths(1);
        }

        return slices;
    }

    private static ReplenishmentRecommendationDto Map(ReplenishmentRecommendation recommendation, Product? product)
    {
        return new ReplenishmentRecommendationDto
        {
            Id = recommendation.Id,
            ProductId = recommendation.ProductId,
            ProductName = product?.Name ?? recommendation.ProductId.ToString(),
            ProductType = product?.ProductType ?? string.Empty,
            ProductReference = product?.Reference ?? string.Empty,
            ProductBrand = product?.Brand ?? string.Empty,
            GeneratedAtUtc = recommendation.GeneratedAtUtc,
            RecommendedForMonth = recommendation.RecommendedForMonth,
            RecommendedUnits = recommendation.RecommendedUnits,
            Confidence = recommendation.Confidence,
            Rationale = recommendation.Rationale,
            Status = recommendation.Status.ToString(),
            ReviewedAtUtc = recommendation.ReviewedAtUtc,
            ReviewedBy = recommendation.ReviewedBy,
            ReviewNotes = recommendation.ReviewNotes
        };
    }
}
