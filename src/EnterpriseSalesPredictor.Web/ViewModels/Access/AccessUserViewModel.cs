namespace EnterpriseSalesPredictor.Web.ViewModels.Access;

public sealed class AccessUserViewModel
{
    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = Array.Empty<string>();
}
