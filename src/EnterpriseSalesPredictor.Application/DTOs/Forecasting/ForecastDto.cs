namespace EnterpriseSalesPredictor.Application.DTOs.Forecasting;

public sealed class ForecastDto
{
    public Guid Id { get; set; }

    public DateTime GeneratedAtUtc { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public decimal ProjectedSales { get; set; }

    public decimal Confidence { get; set; }

    public string GeneratedBy { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public IReadOnlyCollection<CustomerMonthlyForecastDto> CustomerMonthlyForecasts { get; set; } = Array.Empty<CustomerMonthlyForecastDto>();

    public IReadOnlyCollection<ProductMonthlyForecastDto> ProductMonthlyForecasts { get; set; } = Array.Empty<ProductMonthlyForecastDto>();
}

public sealed class CustomerMonthlyForecastDto
{
    public string MonthLabel { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal ProjectedSales { get; set; }

    public decimal Confidence { get; set; }
}

public sealed class ProductMonthlyForecastDto
{
    public string MonthLabel { get; set; } = string.Empty;

    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal ProjectedUnits { get; set; }

    public decimal ProjectedSales { get; set; }

    public decimal Confidence { get; set; }
}
