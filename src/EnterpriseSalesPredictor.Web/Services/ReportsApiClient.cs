using EnterpriseSalesPredictor.Web.ViewModels.Reports;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class ReportsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReportsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ReportViewModel> GetManagementReportAsync(ReportFiltersViewModel filters, CancellationToken cancellationToken = default)
    {
        return GetReportAsync("api/reports/management", filters, cancellationToken);
    }

    public Task<ReportViewModel> GetCommercialReportAsync(ReportFiltersViewModel filters, CancellationToken cancellationToken = default)
    {
        return GetReportAsync("api/reports/commercial", filters, cancellationToken);
    }

    public Task<ReportViewModel> GetOperationalReportAsync(ReportFiltersViewModel filters, CancellationToken cancellationToken = default)
    {
        return GetReportAsync("api/reports/operational", filters, cancellationToken);
    }

    public Task<ReportViewModel> GetReplenishmentReportAsync(ReportFiltersViewModel filters, CancellationToken cancellationToken = default)
    {
        return GetReportAsync("api/reports/replenishment", filters, cancellationToken);
    }

    public Task<ReportViewModel> GetPredictiveReportAsync(ReportFiltersViewModel filters, CancellationToken cancellationToken = default)
    {
        return GetReportAsync("api/reports/predictive", filters, cancellationToken);
    }

    private async Task<ReportViewModel> GetReportAsync(string endpoint, ReportFiltersViewModel filters, CancellationToken cancellationToken)
    {
        AttachBearerToken();

        var parameters = new Dictionary<string, string?>
        {
            [nameof(filters.FromDate)] = filters.FromDate?.ToString("o"),
            [nameof(filters.ToDate)] = filters.ToDate?.ToString("o")
        };

        var url = QueryHelpers.AddQueryString(
            endpoint,
            parameters
                .Where(item => !string.IsNullOrWhiteSpace(item.Value))
                .ToDictionary(item => item.Key, item => item.Value));

        return await _httpClient.GetFromJsonAsync<ReportViewModel>(url, cancellationToken)
            ?? new ReportViewModel();
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
