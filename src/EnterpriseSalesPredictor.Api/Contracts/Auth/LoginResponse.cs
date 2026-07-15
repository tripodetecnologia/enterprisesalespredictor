namespace EnterpriseSalesPredictor.Api.Contracts.Auth;

public sealed class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public int ExpiresInMinutes { get; set; }
}
