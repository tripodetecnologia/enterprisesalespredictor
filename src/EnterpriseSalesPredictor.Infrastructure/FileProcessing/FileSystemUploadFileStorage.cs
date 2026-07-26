using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class FileSystemUploadFileStorage : IUploadFileStorage
{
    private readonly IOptionsMonitor<UploadStorageOptions> _options;

    public FileSystemUploadFileStorage(IOptionsMonitor<UploadStorageOptions> options)
    {
        _options = options;
    }

    public async Task<string> SaveAsync(Guid uploadId, string fileName, Stream stream, CancellationToken cancellationToken = default)
    {
        var rootPath = Path.GetFullPath(_options.CurrentValue.RootPath);
        var uploadDirectory = Path.Combine(rootPath, uploadId.ToString("N"));
        Directory.CreateDirectory(uploadDirectory);

        var safeFileName = Path.GetFileName(fileName);
        var filePath = Path.Combine(uploadDirectory, safeFileName);

        await using var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true);
        await stream.CopyToAsync(fileStream, cancellationToken);
        return filePath;
    }

    public Task DeleteAsync(string filePath, CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
        {
            Directory.Delete(directory);
        }

        return Task.CompletedTask;
    }
}
