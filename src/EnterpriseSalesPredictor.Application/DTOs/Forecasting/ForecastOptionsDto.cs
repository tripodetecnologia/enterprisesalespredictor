namespace EnterpriseSalesPredictor.Application.DTOs.Forecasting;

public sealed class ForecastOptionsDto
{
    public IReadOnlyCollection<ForecastLookupDto> Customers { get; set; } = Array.Empty<ForecastLookupDto>();

    public IReadOnlyCollection<ForecastLookupDto> Products { get; set; } = Array.Empty<ForecastLookupDto>();
}

public sealed class ForecastLookupDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
