namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";

    public string Issuer { get; set; } = "EnterpriseSalesPredictor";

    public string Audience { get; set; } = "EnterpriseSalesPredictor.Clients";

    public string SigningKey { get; set; } = "replace-with-strong-key-at-least-32-characters";

    public int ExpirationMinutes { get; set; } = 60;
}
