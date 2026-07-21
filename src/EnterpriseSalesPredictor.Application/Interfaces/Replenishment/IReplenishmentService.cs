using EnterpriseSalesPredictor.Application.DTOs.Replenishment;

namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public interface IReplenishmentService
{
    Task<PagedReplenishmentProjectionResultDto> GetProjectionsAsync(ReplenishmentProjectionQueryCriteria criteria, CancellationToken cancellationToken = default);

    Task<ReplenishmentRecommendationDto> SubmitProjectionAsync(SubmitReplenishmentProjectionCommand command, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ReplenishmentRecommendationDto>> GenerateRecommendationAsync(GenerateReplenishmentCommand command, CancellationToken cancellationToken = default);

    Task<ReplenishmentRecommendationDto> ReviewRecommendationAsync(ReviewReplenishmentCommand command, CancellationToken cancellationToken = default);

    Task<PagedReplenishmentResultDto> GetRecommendationsAsync(ReplenishmentQueryCriteria criteria, CancellationToken cancellationToken = default);
}
