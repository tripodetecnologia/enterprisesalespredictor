namespace EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;

public sealed class AccessUserDto
{
    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = Array.Empty<string>();
}
