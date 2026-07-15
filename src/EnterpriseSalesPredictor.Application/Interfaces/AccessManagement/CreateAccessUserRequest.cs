namespace EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;

public sealed class CreateAccessUserRequest
{
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = Array.Empty<string>();
}
