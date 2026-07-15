namespace EnterpriseSalesPredictor.Web.ViewModels.Shared;

public sealed class ModalViewModel
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string ConfirmLabel { get; set; } = "Confirm";

    public string CancelLabel { get; set; } = "Cancel";
}
