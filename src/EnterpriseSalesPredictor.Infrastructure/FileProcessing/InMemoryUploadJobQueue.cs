using System.Threading.Channels;
using EnterpriseSalesPredictor.Application.Interfaces.Uploads;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class InMemoryUploadJobQueue : IUploadJobQueue
{
    private readonly Channel<UploadProcessingJob> _channel = Channel.CreateUnbounded<UploadProcessingJob>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = false
    });

    public ValueTask EnqueueAsync(UploadProcessingJob job, CancellationToken cancellationToken = default)
    {
        return _channel.Writer.WriteAsync(job, cancellationToken);
    }

    public ValueTask<UploadProcessingJob> DequeueAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAsync(cancellationToken);
    }
}
