namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class UploadProcessingResult
{
    public Guid UploadId { get; set; }

    public int TotalRecords { get; set; }

    public int ValidRecords { get; set; }

    public int InvalidRecords { get; set; }

    public string Status { get; set; } = string.Empty;
}
