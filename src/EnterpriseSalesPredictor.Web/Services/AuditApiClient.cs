using EnterpriseSalesPredictor.Web.ViewModels.Audit;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class AuditApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyCollection<AuditLogItemViewModel>> GetAuditLogsAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var payload = await _httpClient.GetFromJsonAsync<AuditLogItemViewModel[]>("api/audit", cancellationToken);
        return payload ?? Array.Empty<AuditLogItemViewModel>();
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
