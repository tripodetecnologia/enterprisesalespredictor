namespace EnterpriseSalesPredictor.Web.ViewModels.Forecasting;

public sealed class ForecastPageViewModel
{
    public ForecastRequestViewModel Filters { get; set; } = new();

    public IReadOnlyCollection<ForecastOptionViewModel> Customers { get; set; } = Array.Empty<ForecastOptionViewModel>();

    public IReadOnlyCollection<ForecastOptionViewModel> Products { get; set; } = Array.Empty<ForecastOptionViewModel>();
}

public sealed class ForecastOptionViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
