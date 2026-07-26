namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public interface IUploadJobQueue
{
    ValueTask EnqueueAsync(UploadProcessingJob job, CancellationToken cancellationToken = default);

    ValueTask<UploadProcessingJob> DequeueAsync(CancellationToken cancellationToken = default);
}
