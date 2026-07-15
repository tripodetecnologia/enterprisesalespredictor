namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class CreateUploadSessionCommand
{
    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public string UploadedBy { get; set; } = string.Empty;
}
