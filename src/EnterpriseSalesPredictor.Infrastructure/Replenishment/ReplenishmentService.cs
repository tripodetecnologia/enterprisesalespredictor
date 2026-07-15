using EnterpriseSalesPredictor.Application.DTOs.Replenishment;
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

    public ReplenishmentService(AppDbContext dbContext, IAuditLogService auditLogService)
    {
        _dbContext = dbContext;
        _auditLogService = auditLogService;
    }

    public async Task<ReplenishmentRecommendationDto> GenerateRecommendationAsync(GenerateReplenishmentCommand command, CancellationToken cancellationToken = default)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(item => item.Id == command.ProductId, cancellationToken);
        if (product is null)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.ProductId), "Product not found.")
            });
        }

        var lookbackStart = DateTime.UtcNow.Date.AddDays(-30);
        var sales = await _dbContext.Sales.AsNoTracking()
            .Where(item => item.ProductId == command.ProductId && item.SaleDate >= lookbackStart)
            .ToListAsync(cancellationToken);

        var distinctDays = sales.Select(item => item.SaleDate.Date).Distinct().Count();
        var totalQuantitySold = sales.Sum(item => item.Quantity);
        var averageDailyDemand = distinctDays == 0 ? 0m : totalQuantitySold / distinctDays;

        var projectedDemand = command.ProjectedDemand > 0m ? command.ProjectedDemand : decimal.Round(averageDailyDemand * 30m, 2, MidpointRounding.AwayFromZero);
        var availableUnits = command.AvailableUnits > 0 ? command.AvailableUnits : product.AvailableUnits;
        var shortage = Math.Max(projectedDemand - availableUnits, 0m);
        var lowRotation = totalQuantitySold <= 2m;
        var riskOfStockout = projectedDemand > 0m && availableUnits <= Math.Max(5m, projectedDemand * 0.25m);

        if (!ReplenishmentRules.ShouldGenerateRecommendation(projectedDemand, availableUnits) && !riskOfStockout)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.ProductId), "No replenishment recommendation is needed for the selected product.")
            });
        }

        var recommendedUnits = decimal.Round(Math.Max(shortage, projectedDemand * 0.15m), 2, MidpointRounding.AwayFromZero);
        var confidence = lowRotation ? 0.55m : (riskOfStockout ? 0.82m : 0.72m);

        var rationaleParts = new List<string>
        {
            $"Projected demand for the next 30 days is {projectedDemand:N2} units based on {distinctDays} active sale days.",
            $"Current available stock is {availableUnits} units.",
            $"Estimated replenishment need is {recommendedUnits:N2} units."
        };

        if (riskOfStockout)
        {
            rationaleParts.Add("Stock-out risk detected because available stock covers less than 25% of projected demand.");
        }

        if (lowRotation)
        {
            rationaleParts.Add("Low rotation detected in the last 30 days, so confidence is reduced and the recommendation should be reviewed carefully.");
        }

        var recommendation = new ReplenishmentRecommendation(
            Guid.NewGuid(),
            product.Id,
            DateTime.UtcNow,
            recommendedUnits,
            confidence,
            string.Join(' ', rationaleParts));

        await _dbContext.ReplenishmentRecommendations.AddAsync(recommendation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = command.RequestedBy,
            Action = "RecommendationGenerated",
            Module = "Replenishment",
            Details = $"RecommendationId={recommendation.Id}; ProductId={product.Id}; ProjectedDemand={projectedDemand}; AvailableUnits={availableUnits}; RecommendedUnits={recommendedUnits}; Confidence={confidence}"
        }, cancellationToken);

        return Map(recommendation);
    }

    public async Task<ReplenishmentRecommendationDto> ReviewRecommendationAsync(ReviewReplenishmentCommand command, CancellationToken cancellationToken = default)
    {
        var recommendation = await _dbContext.ReplenishmentRecommendations.FirstOrDefaultAsync(item => item.Id == command.RecommendationId, cancellationToken);
        if (recommendation is null)
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.RecommendationId), "Recommendation not found.")
            });
        }

        if (!ReplenishmentRules.CanApprove(command.ReviewerRole))
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.ReviewerRole), "Reviewer role is not allowed to approve or reject recommendations.")
            });
        }

        var action = command.Action.Trim().ToLowerInvariant();

        if (action == "approve")
        {
            recommendation.Approve(command.Reviewer, command.Notes);
        }
        else if (action == "reject")
        {
            recommendation.Reject(command.Reviewer, command.Notes);
        }
        else if (action == "analysis")
        {
            recommendation.MarkForAnalysis(command.Reviewer, command.Notes);
        }
        else
        {
            throw new ValidationException(new[]
            {
                new ValidationError(nameof(command.Action), "Review action must be approve, reject, or analysis.")
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = command.Reviewer,
            Action = action == "approve" ? "RecommendationApproved" : action == "reject" ? "RecommendationRejected" : "RecommendationMarkedForAnalysis",
            Module = "Replenishment",
            Details = $"RecommendationId={recommendation.Id}; Notes={command.Notes}"
        }, cancellationToken);

        return Map(recommendation);
    }

    public async Task<IReadOnlyCollection<ReplenishmentRecommendationDto>> GetRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        var recommendations = await _dbContext.ReplenishmentRecommendations.AsNoTracking()
            .OrderByDescending(item => item.GeneratedAtUtc)
            .ToListAsync(cancellationToken);

        return recommendations.Select(Map).ToArray();
    }

    private static ReplenishmentRecommendationDto Map(ReplenishmentRecommendation recommendation)
    {
        return new ReplenishmentRecommendationDto
        {
            Id = recommendation.Id,
            ProductId = recommendation.ProductId,
            GeneratedAtUtc = recommendation.GeneratedAtUtc,
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
