namespace EnterpriseSalesPredictor.Web.ViewModels.Access;

public sealed class RolePermissionsViewModel
{
    public string Role { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = Array.Empty<string>();
}
