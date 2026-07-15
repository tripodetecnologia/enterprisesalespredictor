using EnterpriseSalesPredictor.Application.DTOs.Replenishment;

namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public interface IReplenishmentService
{
    Task<ReplenishmentRecommendationDto> GenerateRecommendationAsync(GenerateReplenishmentCommand command, CancellationToken cancellationToken = default);

    Task<ReplenishmentRecommendationDto> ReviewRecommendationAsync(ReviewReplenishmentCommand command, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReplenishmentRecommendationDto>> GetRecommendationsAsync(CancellationToken cancellationToken = default);
}
