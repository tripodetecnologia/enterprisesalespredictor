using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using EnterpriseSalesPredictor.Web.ViewModels.Sales;
using Microsoft.AspNetCore.WebUtilities;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class SalesApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SalesApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyCollection<SaleItemViewModel>> QuerySalesAsync(SalesQueryFilterViewModel filters, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var parameters = new Dictionary<string, string?>
        {
            [nameof(filters.FromDate)] = filters.FromDate?.ToString("o"),
            [nameof(filters.ToDate)] = filters.ToDate?.ToString("o"),
            [nameof(filters.CustomerId)] = filters.CustomerId?.ToString(),
            [nameof(filters.ProductId)] = filters.ProductId?.ToString(),
            [nameof(filters.SupplierId)] = filters.SupplierId?.ToString(),
            [nameof(filters.SellerId)] = filters.SellerId?.ToString(),
            [nameof(filters.City)] = filters.City,
            [nameof(filters.Zone)] = filters.Zone,
            [nameof(filters.PageNumber)] = filters.PageNumber.ToString(),
            [nameof(filters.PageSize)] = filters.PageSize.ToString(),
            [nameof(filters.SortBy)] = filters.SortBy,
            [nameof(filters.SortDirection)] = filters.SortDirection
        };

        var endpoint = QueryHelpers.AddQueryString(
            "api/sales/range",
            parameters
                .Where(item => !string.IsNullOrWhiteSpace(item.Value))
                .ToDictionary(item => item.Key, item => item.Value));
        var payload = await _httpClient.GetFromJsonAsync<SaleItemViewModel[]>(endpoint, cancellationToken);

        return payload ?? Array.Empty<SaleItemViewModel>();
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
