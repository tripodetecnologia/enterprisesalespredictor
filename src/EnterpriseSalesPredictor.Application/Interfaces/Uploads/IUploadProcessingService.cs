namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public interface IUploadProcessingService
{
    Task<UploadProcessingResult> ProcessUploadAsync(
        string fileName,
        string fileType,
        string uploadedBy,
        UploadParseResult parseResult,
        CancellationToken cancellationToken = default);
}
