using EnterpriseSalesPredictor.Web.ViewModels.Auth;

namespace EnterpriseSalesPredictor.Web.Services;

public sealed class WebAuthApiClient : IWebAuthApiClient
{
    private readonly HttpClient _httpClient;

    public WebAuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<LoginResult> LoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
        {
            username,
            password
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return LoginResult.Failure("Credenciales inválidas.");
        }

        var payload = await response.Content.ReadFromJsonAsync<LoginApiResponse>(cancellationToken: cancellationToken);
        if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
        {
            return LoginResult.Failure("El servicio de autenticación devolvió una respuesta inválida.");
        }

        return LoginResult.Success(payload.AccessToken, payload.ExpiresInMinutes);
    }

    private sealed class LoginApiResponse
    {
        public string AccessToken { get; set; } = string.Empty;

        public int ExpiresInMinutes { get; set; }
    }
}
