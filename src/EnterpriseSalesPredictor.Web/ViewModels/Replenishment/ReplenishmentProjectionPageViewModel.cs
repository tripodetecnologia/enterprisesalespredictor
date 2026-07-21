namespace EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

public sealed class ReplenishmentProjectionPageViewModel
{
    public ReplenishmentProjectionFilterViewModel Filters { get; set; } = new();

    public PagedReplenishmentProjectionResultViewModel Results { get; set; } = new();

    public IReadOnlyCollection<Forecasting.ForecastOptionViewModel> Customers { get; set; } = Array.Empty<Forecasting.ForecastOptionViewModel>();

    public IReadOnlyCollection<Forecasting.ForecastOptionViewModel> Products { get; set; } = Array.Empty<Forecasting.ForecastOptionViewModel>();

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
