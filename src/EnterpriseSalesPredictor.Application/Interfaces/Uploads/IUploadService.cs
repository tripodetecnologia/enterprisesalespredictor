namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public interface IUploadService
{
    Task<UploadSessionDto> CreateUploadSessionAsync(CreateUploadSessionCommand command, CancellationToken cancellationToken = default);

    Task<UploadSessionDto?> GetUploadAsync(Guid uploadId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UploadSessionDto>> GetUploadsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<UploadErrorDto>> GetUploadErrorsAsync(Guid uploadId, CancellationToken cancellationToken = default);
}
