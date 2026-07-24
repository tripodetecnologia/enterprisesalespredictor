namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class JwtOptions
{
    public const string SectionName = "Authentication:Jwt";
    public const int MinimumSigningKeyLength = 32;
    public const string DevelopmentSigningKey = "development-only-signing-key-not-for-production";

    public string Issuer { get; set; } = "EnterpriseSalesPredictor";

    public string Audience { get; set; } = "EnterpriseSalesPredictor.Clients";

    public string SigningKey { get; set; } = string.Empty;

    public int ExpirationMinutes { get; set; } = 60;
}
