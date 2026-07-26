namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class UploadProcessingJob
{
    public Guid UploadId { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public string UploadedBy { get; set; } = string.Empty;
}
