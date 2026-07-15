using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class UploadService : IUploadService
{
    private readonly AppDbContext _dbContext;

    public UploadService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UploadSessionDto> CreateUploadSessionAsync(CreateUploadSessionCommand command, CancellationToken cancellationToken = default)
    {
        var upload = new UploadedFile(
            Guid.NewGuid(),
            command.FileName,
            command.FileType,
            DateTime.UtcNow,
            command.UploadedBy,
            UploadProcessStatus.Pending);

        await _dbContext.UploadedFiles.AddAsync(upload, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(upload);
    }

    public async Task<IReadOnlyCollection<UploadSessionDto>> GetUploadsAsync(CancellationToken cancellationToken = default)
    {
        var uploads = await _dbContext.UploadedFiles
            .AsNoTracking()
            .OrderByDescending(upload => upload.UploadedAtUtc)
            .ToListAsync(cancellationToken);

        return uploads.Select(Map).ToArray();
    }

    public async Task<IReadOnlyCollection<UploadErrorDto>> GetUploadErrorsAsync(Guid uploadId, CancellationToken cancellationToken = default)
    {
        var errors = await _dbContext.UploadErrors
            .AsNoTracking()
            .Where(error => error.UploadedFileId == uploadId)
            .OrderBy(error => error.RowNumber)
            .ThenBy(error => error.FieldName)
            .ToListAsync(cancellationToken);

        return errors.Select(MapError).ToArray();
    }

    private static UploadSessionDto Map(UploadedFile upload)
    {
        return new UploadSessionDto
        {
            Id = upload.Id,
            FileName = upload.FileName,
            FileType = upload.FileType,
            UploadedAtUtc = upload.UploadedAtUtc,
            UploadedBy = upload.UploadedBy,
            Status = upload.Status.ToString(),
            TotalRecords = upload.TotalRecords,
            ValidRecords = upload.ValidRecords,
            InvalidRecords = upload.InvalidRecords
        };
    }

    private static UploadErrorDto MapError(UploadError error)
    {
        return new UploadErrorDto
        {
            Id = error.Id,
            UploadId = error.UploadedFileId,
            RowNumber = error.RowNumber,
            FieldName = error.FieldName,
            ErrorMessage = error.ErrorMessage
        };
    }
}
