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
        if (!TryValidateFile(file, UploadPolicy.ExcelExtensions, out var errorMessage))
        {
            return BadRequest(new { message = errorMessage });
        }

        var response = await ProcessAsync(file, cancellationToken);
        return Ok(response);
    }

    [HttpPost("delimited")]
    [Authorize(Policy = PermissionPolicies.UploadsWrite)]
    public async Task<ActionResult<UploadProcessResponse>> UploadDelimitedAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (!TryValidateFile(file, UploadPolicy.DelimitedExtensions, out var errorMessage))
        {
            return BadRequest(new { message = errorMessage });
        }

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
            throw new InvalidOperationException("No parser available for a validated file.");
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
