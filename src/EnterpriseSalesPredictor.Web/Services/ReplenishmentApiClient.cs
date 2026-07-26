using EnterpriseSalesPredictor.Web.ViewModels.Forecasting;
using EnterpriseSalesPredictor.Web.ViewModels.Replenishment;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class ReplenishmentApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReplenishmentApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedReplenishmentProjectionResultViewModel> GetProjectionsAsync(ReplenishmentProjectionFilterViewModel filters, int pageNumber, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var endpoint = QueryHelpers.AddQueryString("api/replenishment/projections", new Dictionary<string, string?>
        {
            ["FromDate"] = filters.FromDate?.ToString("o"),
            ["ToDate"] = filters.ToDate?.ToString("o"),
            ["CustomerId"] = filters.CustomerId?.ToString(),
            ["ProductName"] = filters.ProductName,
            ["PageNumber"] = pageNumber.ToString(),
            ["PageSize"] = "10"
        }.Where(item => !string.IsNullOrWhiteSpace(item.Value)).ToDictionary(item => item.Key, item => item.Value));

        return await _httpClient.GetFromJsonAsync<PagedReplenishmentProjectionResultViewModel>(endpoint, cancellationToken)
            ?? new PagedReplenishmentProjectionResultViewModel();
    }

    public async Task<PagedReplenishmentResultViewModel> GetRecommendationsAsync(string status, DateTime? fromDate, DateTime? toDate, Guid? productId, int pageNumber, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var endpoint = QueryHelpers.AddQueryString("api/replenishment/recommendations", new Dictionary<string, string?>
        {
            ["Status"] = status,
            ["FromDate"] = fromDate?.ToString("o"),
            ["ToDate"] = toDate?.ToString("o"),
            ["ProductId"] = productId?.ToString(),
            ["PageNumber"] = pageNumber.ToString(),
            ["PageSize"] = "10"
        }.Where(item => !string.IsNullOrWhiteSpace(item.Value)).ToDictionary(item => item.Key, item => item.Value));

        return await _httpClient.GetFromJsonAsync<PagedReplenishmentResultViewModel>(endpoint, cancellationToken)
            ?? new PagedReplenishmentResultViewModel();
    }

    public async Task SubmitProjectionAsync(ReplenishmentProjectionViewModel projection, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var response = await _httpClient.PostAsJsonAsync("api/replenishment/projections/submit", new
        {
            projectionMonth = projection.ProjectionMonth,
            productId = projection.ProductId,
            recommendedUnits = projection.RecommendedUnits,
            currentStockUnits = projection.CurrentStockUnits
        }, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Recommendation API failed: {content}");
        }
    }

    public async Task<(IReadOnlyCollection<ForecastOptionViewModel> Customers, IReadOnlyCollection<ForecastOptionViewModel> Products)> GetOptionsAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var payload = await _httpClient.GetFromJsonAsync<ForecastOptionsResponse>("api/forecasts/options", cancellationToken);
        if (payload is null)
        {
            return (Array.Empty<ForecastOptionViewModel>(), Array.Empty<ForecastOptionViewModel>());
        }

        return (
            payload.Customers.Select(item => new ForecastOptionViewModel { Id = item.Id, Name = item.Name }).ToArray(),
            payload.Products.Select(item => new ForecastOptionViewModel { Id = item.Id, Name = item.Name }).ToArray());
    }

    public async Task ReviewRecommendationAsync(Guid recommendationId, string action, string? notes, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var response = await _httpClient.PostAsJsonAsync($"api/replenishment/recommendations/{recommendationId}/{action}", new { notes }, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(ParseApiError(content, "No fue posible procesar la sugerencia."));
        }
    }

    private static string ParseApiError(string content, string fallbackMessage)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return fallbackMessage;
        }

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in errors.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array && property.Value.GetArrayLength() > 0)
                    {
                        var message = property.Value[0].GetString();
                        if (!string.IsNullOrWhiteSpace(message))
                        {
                            return message;
                        }
                    }
                }
            }

            if (root.TryGetProperty("message", out var messageProperty))
            {
                var message = messageProperty.GetString();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return message;
                }
            }

            if (root.TryGetProperty("detail", out var detailProperty))
            {
                var detail = detailProperty.GetString();
                if (!string.IsNullOrWhiteSpace(detail))
                {
                    return detail;
                }
            }

            if (root.TryGetProperty("title", out var titleProperty))
            {
                var title = titleProperty.GetString();
                if (!string.IsNullOrWhiteSpace(title))
                {
                    return title;
                }
            }
        }
        catch (JsonException)
        {
        }

        return fallbackMessage;
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
