using EnterpriseSalesPredictor.Application.DTOs.Replenishment;

namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public sealed class PagedReplenishmentResultDto
{
    public IReadOnlyCollection<ReplenishmentRecommendationDto> Items { get; set; } = Array.Empty<ReplenishmentRecommendationDto>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
