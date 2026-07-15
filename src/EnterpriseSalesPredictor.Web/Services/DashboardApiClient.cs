using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using EnterpriseSalesPredictor.Web.ViewModels.Dashboard;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class DashboardApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<DashboardKpiViewModel> GetKpisAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        return await _httpClient.GetFromJsonAsync<DashboardKpiViewModel>("api/dashboard/kpis", cancellationToken)
            ?? new DashboardKpiViewModel();
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemViewModel>> GetTopCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await GetBreakdownAsync("api/dashboard/top-customers", cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemViewModel>> GetTopProductsAsync(CancellationToken cancellationToken = default)
    {
        return await GetBreakdownAsync("api/dashboard/top-products", cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemViewModel>> GetSalesByLineAsync(CancellationToken cancellationToken = default)
    {
        return await GetBreakdownAsync("api/dashboard/sales-by-line", cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardBreakdownItemViewModel>> GetSalesBySupplierAsync(CancellationToken cancellationToken = default)
    {
        return await GetBreakdownAsync("api/dashboard/sales-by-supplier", cancellationToken);
    }

    public async Task<IReadOnlyCollection<DashboardAlertViewModel>> GetAlertsAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var payload = await _httpClient.GetFromJsonAsync<DashboardAlertViewModel[]>("api/dashboard/alerts", cancellationToken);
        return payload ?? Array.Empty<DashboardAlertViewModel>();
    }

    private async Task<IReadOnlyCollection<DashboardBreakdownItemViewModel>> GetBreakdownAsync(string endpoint, CancellationToken cancellationToken)
    {
        AttachBearerToken();
        var payload = await _httpClient.GetFromJsonAsync<DashboardBreakdownItemViewModel[]>(endpoint, cancellationToken);
        return payload ?? Array.Empty<DashboardBreakdownItemViewModel>();
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
