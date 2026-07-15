using System.ComponentModel.DataAnnotations;

namespace EnterpriseSalesPredictor.Web.ViewModels.Auth;

public sealed class LoginViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public string? ReturnUrl { get; set; }
}
