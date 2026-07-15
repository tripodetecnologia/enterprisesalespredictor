using EnterpriseSalesPredictor.Application.DTOs.Forecasting;

namespace EnterpriseSalesPredictor.Application.Interfaces.Forecasting;

public interface IForecastService
{
    Task<ForecastDto> GenerateForecastAsync(ForecastQuery query, CancellationToken cancellationToken = default);
}
