using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using EnterpriseSalesPredictor.Web.ViewModels.Access;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class AccessManagementApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccessManagementApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyCollection<AccessUserViewModel>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var payload = await _httpClient.GetFromJsonAsync<AccessUserViewModel[]>("api/access/users", cancellationToken);
        return payload ?? Array.Empty<AccessUserViewModel>();
    }

    public async Task<IReadOnlyCollection<RolePermissionsViewModel>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var payload = await _httpClient.GetFromJsonAsync<RolePermissionsViewModel[]>("api/access/roles", cancellationToken);
        return payload ?? Array.Empty<RolePermissionsViewModel>();
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionCatalogAsync(CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var payload = await _httpClient.GetFromJsonAsync<string[]>("api/access/permissions", cancellationToken);
        return payload ?? Array.Empty<string>();
    }

    public async Task CreateUserAsync(CreateAccessUserFormViewModel model, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var response = await _httpClient.PostAsJsonAsync("api/access/users", new
        {
            model.Username,
            model.Password,
            model.Role,
            Permissions = model.Permissions
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"La API rechazó la creación del usuario: {content}");
        }
    }

    public async Task UpdateRolePermissionsAsync(UpdateRolePermissionsFormViewModel model, CancellationToken cancellationToken = default)
    {
        AttachBearerToken();

        var response = await _httpClient.PutAsJsonAsync("api/access/roles/permissions", new
        {
            model.Role,
            Permissions = model.Permissions
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"La API rechazó la actualización del rol: {content}");
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
