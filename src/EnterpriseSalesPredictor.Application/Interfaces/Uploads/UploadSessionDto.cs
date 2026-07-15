namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class UploadSessionDto
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public DateTime UploadedAtUtc { get; set; }

    public string UploadedBy { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int TotalRecords { get; set; }

    public int ValidRecords { get; set; }

    public int InvalidRecords { get; set; }
}
