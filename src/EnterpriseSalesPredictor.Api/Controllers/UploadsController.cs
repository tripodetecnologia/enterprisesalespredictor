using EnterpriseSalesPredictor.Api.Contracts.Uploads;
using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Application.Interfaces.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize]
public sealed class UploadsController : ControllerBase
{
    private readonly IEnumerable<IUploadFileParser> _parsers;
    private readonly IUploadService _uploadService;
    private readonly IUploadFileStorage _uploadFileStorage;
    private readonly IUploadJobQueue _uploadJobQueue;
    private readonly IAuditLogService _auditLogService;

    public UploadsController(
        IEnumerable<IUploadFileParser> parsers,
        IUploadService uploadService,
        IUploadFileStorage uploadFileStorage,
        IUploadJobQueue uploadJobQueue,
        IAuditLogService auditLogService)
    {
        _parsers = parsers;
        _uploadService = uploadService;
        _uploadFileStorage = uploadFileStorage;
        _uploadJobQueue = uploadJobQueue;
        _auditLogService = auditLogService;
    }

    [HttpPost("excel")]
    [Authorize(Policy = PermissionPolicies.UploadsWrite)]
    public async Task<ActionResult<UploadProcessResponse>> UploadExcelAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (!TryValidateFile(file, UploadPolicy.ExcelExtensions, out var errorMessage))
        {
            return BadRequest(new { message = errorMessage });
        }

        var response = await QueueAsync(file, UploadPolicy.ExcelParserKey, cancellationToken);
        return Accepted($"/api/uploads/{response.UploadId}", response);
    }

    [HttpPost("delimited")]
    [Authorize(Policy = PermissionPolicies.UploadsWrite)]
    public async Task<ActionResult<UploadProcessResponse>> UploadDelimitedAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (!TryValidateFile(file, UploadPolicy.DelimitedExtensions, out var errorMessage))
        {
            return BadRequest(new { message = errorMessage });
        }

        var response = await QueueAsync(file, UploadPolicy.DelimitedParserKey, cancellationToken);
        return Accepted($"/api/uploads/{response.UploadId}", response);
    }

    [HttpGet]
    [Authorize(Policy = PermissionPolicies.UploadsRead)]
    public async Task<IActionResult> GetUploadsAsync(CancellationToken cancellationToken)
    {
        var uploads = await _uploadService.GetUploadsAsync(cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = User.Identity?.Name ?? "system",
            Action = "UploadHistoryViewed",
            Module = "Uploads",
            Details = $"Entries={uploads.Count}"
        }, cancellationToken);

        return Ok(uploads);
    }

    [HttpGet("{uploadId:guid}")]
    [Authorize(Policy = PermissionPolicies.UploadsRead)]
    public async Task<IActionResult> GetUploadAsync(Guid uploadId, CancellationToken cancellationToken)
    {
        var upload = await _uploadService.GetUploadAsync(uploadId, cancellationToken);
        return upload is null ? NotFound() : Ok(upload);
    }

    [HttpGet("{uploadId:guid}/errors")]
    [Authorize(Policy = PermissionPolicies.UploadsRead)]
    public async Task<IActionResult> GetUploadErrorsAsync(Guid uploadId, CancellationToken cancellationToken)
    {
        var errors = await _uploadService.GetUploadErrorsAsync(uploadId, cancellationToken);
        return Ok(errors);
    }

    private async Task<UploadProcessResponse> QueueAsync(IFormFile file, string fileType, CancellationToken cancellationToken)
    {
        var parser = _parsers.FirstOrDefault(candidate => candidate.ParserKey == fileType && candidate.CanHandle(file.FileName));
        if (parser is null)
        {
            throw new InvalidOperationException("No parser available for a validated file.");
        }

        var uploadedBy = User.Identity?.Name ?? "system";
        var uploadSession = await _uploadService.CreateUploadSessionAsync(new CreateUploadSessionCommand
        {
            FileName = file.FileName,
            FileType = parser.ParserKey,
            UploadedBy = uploadedBy
        }, cancellationToken);

        await using var stream = file.OpenReadStream();
        var filePath = await _uploadFileStorage.SaveAsync(uploadSession.Id, file.FileName, stream, cancellationToken);

        await _uploadJobQueue.EnqueueAsync(new UploadProcessingJob
        {
            UploadId = uploadSession.Id,
            FilePath = filePath,
            FileName = file.FileName,
            FileType = parser.ParserKey,
            UploadedBy = uploadedBy
        }, cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = uploadedBy,
            Action = "UploadQueued",
            Module = "Uploads",
            Details = $"UploadId={uploadSession.Id}; File={file.FileName}; Type={parser.ParserKey}"
        }, cancellationToken);

        return new UploadProcessResponse
        {
            UploadId = uploadSession.Id,
            Status = uploadSession.Status
        };
    }

    private static bool TryValidateFile(IFormFile? file, IReadOnlyCollection<string> allowedExtensions, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (file is null)
        {
            errorMessage = "A file is required.";
            return false;
        }

        if (file.Length <= 0)
        {
            errorMessage = "The file is empty.";
            return false;
        }

        if (file.Length > UploadPolicy.MaxFileSizeBytes)
        {
            errorMessage = $"The file exceeds the maximum allowed size ({UploadPolicy.MaxFileSizeBytes} bytes).";
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            errorMessage = $"Invalid file extension: {extension}.";
            return false;
        }

        return true;
    }
}
