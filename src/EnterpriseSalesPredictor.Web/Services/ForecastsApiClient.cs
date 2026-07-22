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
            request.ProductName,
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

    public async Task<ForecastPageViewModel> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var payload = await _httpClient.GetFromJsonAsync<ForecastOptionsResponse>("api/forecasts/options", cancellationToken);
        if (payload is null)
        {
            return new ForecastPageViewModel();
        }

        return new ForecastPageViewModel
        {
            Customers = payload.Customers
                .Select(item => new ForecastOptionViewModel { Id = item.Id, Name = item.Name })
                .ToArray(),
            Products = payload.Products
                .Select(item => new ForecastOptionViewModel { Id = item.Id, Name = item.Name })
                .GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(item => item.Name)
                .ToArray()
        };
    }

    private void AttachBearerToken()
    {
        var accessToken = _httpContextAccessor.HttpContext?.User.FindFirstValue("access_token");
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    private sealed class ForecastOptionsResponse
    {
        public ForecastOptionResponse[] Customers { get; set; } = Array.Empty<ForecastOptionResponse>();

        public ForecastOptionResponse[] Products { get; set; } = Array.Empty<ForecastOptionResponse>();
    }

    private sealed class ForecastOptionResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
