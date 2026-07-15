using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using EnterpriseSalesPredictor.Web.ViewModels.Forecasting;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class ForecastsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ForecastsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ForecastResultViewModel> GenerateForecastAsync(ForecastRequestViewModel request, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var response = await _httpClient.PostAsJsonAsync("api/forecasts", new
        {
            request.FromDate,
            request.ToDate,
            request.ProductId,
            request.CustomerId
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Forecast API failed: {content}");
        }

        var payload = await response.Content.ReadFromJsonAsync<ForecastResultViewModel>(cancellationToken: cancellationToken);
        if (payload is null)
        {
            throw new InvalidOperationException("Forecast API returned an invalid response.");
        }

        return payload;
    }

    private void AttachBearerToken()
    {
        var accessToken = _httpContextAccessor.HttpContext?.User.FindFirstValue("access_token");
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
