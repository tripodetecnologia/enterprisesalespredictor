namespace EnterpriseSalesPredictor.Application.Interfaces.AccessManagement;

public sealed class RolePermissionsDto
{
    public string Role { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = Array.Empty<string>();
}
