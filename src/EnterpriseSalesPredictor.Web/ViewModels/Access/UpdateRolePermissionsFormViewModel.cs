using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Access;

public sealed class UpdateRolePermissionsFormViewModel
{
    [Required]
    public string Role { get; set; } = string.Empty;

    public List<string> Permissions { get; set; } = new();

    public string PermissionsRaw { get; set; } = string.Empty;
}
