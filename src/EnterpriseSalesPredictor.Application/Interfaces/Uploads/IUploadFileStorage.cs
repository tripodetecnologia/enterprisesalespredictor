namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public interface IUploadFileStorage
{
    Task<string> SaveAsync(Guid uploadId, string fileName, Stream stream, CancellationToken cancellationToken = default);

    Task DeleteAsync(string filePath, CancellationToken cancellationToken = default);
}
