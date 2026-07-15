using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Api.Contracts.Access;

public sealed class CreateUserRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;

    public IReadOnlyCollection<string> Permissions { get; set; } = Array.Empty<string>();
}
