namespace EnterpriseSalesPredictor.Web.ViewModels.Forecasting;

public sealed class ForecastResultViewModel
{
    public Guid Id { get; set; }

    public DateTime GeneratedAtUtc { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public decimal ProjectedSales { get; set; }

    public decimal Confidence { get; set; }

    public string GeneratedBy { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public IReadOnlyCollection<CustomerMonthlyForecastViewModel> CustomerMonthlyForecasts { get; set; } = Array.Empty<CustomerMonthlyForecastViewModel>();

    public IReadOnlyCollection<ProductMonthlyForecastViewModel> ProductMonthlyForecasts { get; set; } = Array.Empty<ProductMonthlyForecastViewModel>();
}

public sealed class CustomerMonthlyForecastViewModel
{
    public string MonthLabel { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal ProjectedSales { get; set; }

    public decimal Confidence { get; set; }
}

public sealed class ProductMonthlyForecastViewModel
{
    public string MonthLabel { get; set; } = string.Empty;

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal ProjectedUnits { get; set; }

    public decimal ProjectedSales { get; set; }

    public decimal Confidence { get; set; }
}
