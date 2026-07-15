using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Access;

public sealed class CreateAccessUserFormViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    public List<string> Permissions { get; set; } = new();

    public string PermissionsRaw { get; set; } = string.Empty;
}
