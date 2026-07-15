using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using EnterpriseSalesPredictor.Web.ViewModels.Replenishment;

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

    public async Task<IReadOnlyCollection<ReplenishmentRecommendationViewModel>> GetRecommendationsAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var payload = await _httpClient.GetFromJsonAsync<ReplenishmentRecommendationViewModel[]>("api/replenishment/recommendations", cancellationToken);
        return payload ?? Array.Empty<ReplenishmentRecommendationViewModel>();
    }

    public async Task GenerateRecommendationAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var response = await _httpClient.PostAsJsonAsync("api/replenishment/recommendations", new { productId }, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Recommendation API failed: {content}");
        }
    }

    public async Task ReviewRecommendationAsync(Guid recommendationId, string action, string? notes, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var response = await _httpClient.PostAsJsonAsync($"api/replenishment/recommendations/{recommendationId}/{action}", new { notes }, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Recommendation review failed: {content}");
        }
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
