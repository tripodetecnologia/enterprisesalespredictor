using EnterpriseSalesPredictor.Application.DTOs.Replenishment;

namespace EnterpriseSalesPredictor.Application.Interfaces.Replenishment;

public sealed class ReplenishmentProjectionQueryCriteria
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public Guid? CustomerId { get; set; }

    public string? ProductName { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

public sealed class PagedReplenishmentProjectionResultDto
{
    public IReadOnlyCollection<ReplenishmentProjectionDto> Items { get; set; } = Array.Empty<ReplenishmentProjectionDto>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
