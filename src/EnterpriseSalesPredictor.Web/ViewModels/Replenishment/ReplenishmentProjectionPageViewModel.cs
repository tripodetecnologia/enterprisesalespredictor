namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentProjectionPageViewModel
{
    public ReplenishmentProjectionFilterViewModel Filters { get; set; } = new();

    public PagedReplenishmentProjectionResultViewModel Results { get; set; } = new();

    public IReadOnlyCollection<Forecasting.ForecastOptionViewModel> Customers { get; set; } = Array.Empty<Forecasting.ForecastOptionViewModel>();

    public IReadOnlyCollection<ReplenishmentProductOptionViewModel> Products { get; set; } = Array.Empty<ReplenishmentProductOptionViewModel>();

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}

public sealed class ReplenishmentProductOptionViewModel
{
    public string Name { get; set; } = string.Empty;
}
