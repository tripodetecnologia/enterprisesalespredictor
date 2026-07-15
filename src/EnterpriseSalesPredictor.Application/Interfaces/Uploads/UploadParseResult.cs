namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class UploadParseResult
{
    public List<UploadRecordData> Records { get; set; } = new();

    public List<UploadParseError> Errors { get; set; } = new();
}
