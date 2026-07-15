namespace EnterpriseSalesPredictor.Web.ViewModels.Uploads;

public sealed class UploadProcessingResponseViewModel
{
    public Guid UploadId { get; set; }

    public int TotalRecords { get; set; }

    public int ValidRecords { get; set; }

    public int InvalidRecords { get; set; }

    public string Status { get; set; } = string.Empty;
}
