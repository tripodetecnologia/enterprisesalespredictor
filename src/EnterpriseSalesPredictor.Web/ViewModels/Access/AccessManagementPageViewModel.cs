namespace EnterpriseSalesPredictor.Web.ViewModels.Access;

public sealed class AccessManagementPageViewModel
{
    public IReadOnlyCollection<AccessUserViewModel> Users { get; set; } = Array.Empty<AccessUserViewModel>();

    public IReadOnlyCollection<RolePermissionsViewModel> Roles { get; set; } = Array.Empty<RolePermissionsViewModel>();

    public IReadOnlyCollection<string> PermissionCatalog { get; set; } = Array.Empty<string>();

    public CreateAccessUserFormViewModel CreateUserForm { get; set; } = new();

    public UpdateRolePermissionsFormViewModel UpdateRoleForm { get; set; } = new();

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
