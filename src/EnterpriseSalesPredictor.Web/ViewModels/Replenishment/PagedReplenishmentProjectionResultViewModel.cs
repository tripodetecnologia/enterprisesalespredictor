namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class PagedReplenishmentProjectionResultViewModel
{
    public IReadOnlyCollection<ReplenishmentProjectionViewModel> Items { get; set; } = Array.Empty<ReplenishmentProjectionViewModel>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
