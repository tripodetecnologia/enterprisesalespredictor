using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Api.Contracts.Access;

public sealed class UpdateRolePermissionsRequest
{
    [Required]
    public string Role { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = Array.Empty<string>();
}
