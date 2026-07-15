namespace EnterpriseSalesPredictor.Web.ViewModels.Uploads;

public sealed class UploadPageViewModel
{
    public IReadOnlyCollection<UploadSessionViewModel> Uploads { get; set; } = Array.Empty<UploadSessionViewModel>();

    public UploadProcessingResponseViewModel? LastResult { get; set; }

    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; set; }
}
