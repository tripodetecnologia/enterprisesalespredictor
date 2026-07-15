namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class AuthSeedOptions
{
    public const string SectionName = "Authentication:Users";

    public List<AuthSeedUser> Users { get; set; } = new();
}

public sealed class AuthSeedUser
{
    public string UserId { get; set; } = Guid.NewGuid().ToString("N");

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public List<string> Permissions { get; set; } = new();
}
