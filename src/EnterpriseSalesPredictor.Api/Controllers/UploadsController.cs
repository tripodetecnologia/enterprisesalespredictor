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
    private readonly IUploadProcessingService _uploadProcessingService;
    private readonly IUploadService _uploadService;
    private readonly IAuditLogService _auditLogService;

    public UploadsController(
        IEnumerable<IUploadFileParser> parsers,
        IUploadProcessingService uploadProcessingService,
        IUploadService uploadService,
        IAuditLogService auditLogService)
    {
        _parsers = parsers;
        _uploadProcessingService = uploadProcessingService;
        _uploadService = uploadService;
        _auditLogService = auditLogService;
    }

    [HttpPost("excel")]
    [Authorize(Policy = PermissionPolicies.UploadsWrite)]
    public async Task<ActionResult<UploadProcessResponse>> UploadExcelAsync(IFormFile file, CancellationToken cancellationToken)
    {
        ValidateFile(file, UploadPolicy.ExcelExtensions);
        var response = await ProcessAsync(file, cancellationToken);
        return Ok(response);
    }

    [HttpPost("delimited")]
    [Authorize(Policy = PermissionPolicies.UploadsWrite)]
    public async Task<ActionResult<UploadProcessResponse>> UploadDelimitedAsync(IFormFile file, CancellationToken cancellationToken)
    {
        ValidateFile(file, UploadPolicy.DelimitedExtensions);
        var response = await ProcessAsync(file, cancellationToken);
        return Ok(response);
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

    [HttpGet("{uploadId:guid}/errors")]
    [Authorize(Policy = PermissionPolicies.UploadsRead)]
    public async Task<IActionResult> GetUploadErrorsAsync(Guid uploadId, CancellationToken cancellationToken)
    {
        var errors = await _uploadService.GetUploadErrorsAsync(uploadId, cancellationToken);
        return Ok(errors);
    }

    private async Task<UploadProcessResponse> ProcessAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var parser = _parsers.FirstOrDefault(candidate => candidate.CanHandle(file.FileName));
        if (parser is null)
        {
            throw new InvalidOperationException("No parser available for the selected file.");
        }

        await using var stream = file.OpenReadStream();
        var parseResult = await parser.ParseAsync(stream, cancellationToken);

        var uploadedBy = User.Identity?.Name ?? "system";
        var result = await _uploadProcessingService.ProcessUploadAsync(
            file.FileName,
            parser.ParserKey,
            uploadedBy,
            parseResult,
            cancellationToken);

        await _auditLogService.RecordAsync(new CreateAuditLogCommand
        {
            Actor = uploadedBy,
            Action = "UploadProcessed",
            Module = "Uploads",
            Details = $"UploadId={result.UploadId}; File={file.FileName}; Status={result.Status}; Total={result.TotalRecords}; Valid={result.ValidRecords}; Invalid={result.InvalidRecords}"
        }, cancellationToken);

        return new UploadProcessResponse
        {
            UploadId = result.UploadId,
            TotalRecords = result.TotalRecords,
            ValidRecords = result.ValidRecords,
            InvalidRecords = result.InvalidRecords,
            Status = result.Status
        };
    }

    private static void ValidateFile(IFormFile? file, IReadOnlyCollection<string> allowedExtensions)
    {
        if (file is null)
        {
            throw new InvalidOperationException("A file is required.");
        }

        if (file.Length <= 0)
        {
            throw new InvalidOperationException("The file is empty.");
        }

        if (file.Length > UploadPolicy.MaxFileSizeBytes)
        {
            throw new InvalidOperationException($"The file exceeds the maximum allowed size ({UploadPolicy.MaxFileSizeBytes} bytes).");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Invalid file extension: {extension}.");
        }
    }
}
