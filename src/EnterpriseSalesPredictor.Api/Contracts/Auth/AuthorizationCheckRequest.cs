namespace EnterpriseSalesPredictor.Api.Contracts.Auth;

public sealed class AuthorizationCheckRequest
{
    public string Module { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;
}
