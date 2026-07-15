namespace EnterpriseSalesPredictor.Web.ViewModels.Shared;

public sealed class AlertViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Variant { get; set; } = "info";
}
