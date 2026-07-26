using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class ExportsApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExportsApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ExportDownloadResult> ExportReportsAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        return DownloadAsync("api/exports/reports", fromDate, toDate, cancellationToken);
    }

    public Task<ExportDownloadResult> ExportFilteredSalesAsync(IReadOnlyDictionary<string, string?> query, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        var endpoint = QueryHelpers.AddQueryString(
            "api/exports/filtered-sales",
            query.Where(item => !string.IsNullOrWhiteSpace(item.Value)).ToDictionary(item => item.Key, item => item.Value));

        return DownloadAsync(endpoint, cancellationToken);
    }

    public Task<ExportDownloadResult> ExportBaseDataAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();
        return DownloadAsync("api/exports/base-data", cancellationToken);
    }

    private Task<ExportDownloadResult> DownloadAsync(string endpoint, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        AttachBearerToken();
        var parameters = new Dictionary<string, string?>
        {
            ["FromDate"] = fromDate?.ToString("o"),
            ["ToDate"] = toDate?.ToString("o")
        };

        var url = QueryHelpers.AddQueryString(
            endpoint,
            parameters.Where(item => !string.IsNullOrWhiteSpace(item.Value)).ToDictionary(item => item.Key, item => item.Value));

        return DownloadAsync(url, cancellationToken);
    }

    private async Task<ExportDownloadResult> DownloadAsync(string endpoint, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Export API failed: {content}");
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString()
            ?? ExportFormats.ExcelContentType;
        var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
            ?? response.Content.Headers.ContentDisposition?.FileName
            ?? $"export-{DateTime.UtcNow.ToString(DateFormats.ExportTimestamp)}{ExportFormats.ExcelExtension}";

        return new ExportDownloadResult(bytes, contentType, fileName.Trim('"'));
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

public sealed record ExportDownloadResult(byte[] Content, string ContentType, string FileName);
