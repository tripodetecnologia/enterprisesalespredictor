using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class BackgroundUploadProcessingService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUploadJobQueue _queue;
    private readonly ILogger<BackgroundUploadProcessingService> _logger;

    public BackgroundUploadProcessingService(
        IServiceScopeFactory scopeFactory,
        IUploadJobQueue queue,
        ILogger<BackgroundUploadProcessingService> logger)
    {
        _scopeFactory = scopeFactory;
        _queue = queue;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _queue.DequeueAsync(stoppingToken);
            await ProcessJobAsync(job, stoppingToken);
        }
    }

    private async Task ProcessJobAsync(UploadProcessingJob job, CancellationToken cancellationToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var processor = scope.ServiceProvider.GetRequiredService<IUploadProcessingService>();
        var auditLogService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
        var storage = scope.ServiceProvider.GetRequiredService<IUploadFileStorage>();

        try
        {
            var result = await processor.ProcessStoredUploadAsync(
                job.UploadId,
                job.FilePath,
                job.FileName,
                job.FileType,
                job.UploadedBy,
                cancellationToken);

            await auditLogService.RecordAsync(new CreateAuditLogCommand
            {
                Actor = job.UploadedBy,
                Action = "UploadProcessed",
                Module = "Uploads",
                Details = $"UploadId={result.UploadId}; File={job.FileName}; Status={result.Status}; Total={result.TotalRecords}; Valid={result.ValidRecords}; Invalid={result.InvalidRecords}"
            }, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Upload job {UploadId} was canceled because the host is stopping.", job.UploadId);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Upload job {UploadId} failed.", job.UploadId);
            await auditLogService.RecordAsync(new CreateAuditLogCommand
            {
                Actor = job.UploadedBy,
                Action = "UploadFailed",
                Module = "Uploads",
                Details = $"UploadId={job.UploadId}; File={job.FileName}; Error={exception.Message}"
            }, cancellationToken);
        }
        finally
        {
            await storage.DeleteAsync(job.FilePath, cancellationToken);
        }
    }
}
