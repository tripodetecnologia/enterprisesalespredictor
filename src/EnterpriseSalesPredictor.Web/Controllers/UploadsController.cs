using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
public sealed class UploadsController : Controller
{
    private const long MaxUploadSizeBytes = 20 * 1024 * 1024;
    private static readonly string[] ExcelExtensions = { ".xlsx", ".xls" };
    private static readonly string[] DelimitedExtensions = { ".csv", ".txt" };

    private readonly UploadsApiClient _uploadsApiClient;

    public UploadsController(UploadsApiClient uploadsApiClient)
    {
        _uploadsApiClient = uploadsApiClient;
    }

    [HttpGet]
    [RequirePermission("uploads:read")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await BuildPageModelAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("uploads:write")]
    public async Task<IActionResult> Upload(IFormFile? file, CancellationToken cancellationToken)
    {
        if (!TryValidateFile(file, out var fileType, out var errorMessage))
        {
            var invalidModel = await BuildPageModelAsync(cancellationToken);
            invalidModel.ErrorMessage = errorMessage;
            return View("Index", invalidModel);
        }

        try
        {
            await using var stream = file!.OpenReadStream();
            UploadProcessingResponseViewModel result;

            if (fileType == "excel")
            {
                result = await _uploadsApiClient.UploadExcelAsync(stream, file.FileName, cancellationToken);
            }
            else
            {
                result = await _uploadsApiClient.UploadDelimitedAsync(stream, file.FileName, cancellationToken);
            }

            var viewModel = await BuildPageModelAsync(cancellationToken);
            viewModel.LastResult = result;
            viewModel.StatusMessage = "Upload completed.";
            return View("Index", viewModel);
        }
        catch (Exception exception)
        {
            var errorModel = await BuildPageModelAsync(cancellationToken);
            errorModel.ErrorMessage = exception.Message;
            return View("Index", errorModel);
        }
    }

    [HttpGet]
    [RequirePermission("uploads:read")]
    public async Task<IActionResult> Errors(Guid id, CancellationToken cancellationToken)
    {
        var errors = await _uploadsApiClient.GetUploadErrorsAsync(id, cancellationToken);
        ViewData["Title"] = "Upload Error Details";
        ViewData["UploadId"] = id;
        return View(errors);
    }

    private async Task<UploadPageViewModel> BuildPageModelAsync(CancellationToken cancellationToken)
    {
        var uploads = await _uploadsApiClient.GetUploadsAsync(cancellationToken);
        return new UploadPageViewModel
        {
            Uploads = uploads
        };
    }

    private static bool TryValidateFile(IFormFile? file, out string fileType, out string errorMessage)
    {
        fileType = string.Empty;
        errorMessage = string.Empty;

        if (file is null)
        {
            errorMessage = "Please select a file to upload.";
            return false;
        }

        if (file.Length <= 0)
        {
            errorMessage = "Selected file is empty.";
            return false;
        }

        if (file.Length > MaxUploadSizeBytes)
        {
            errorMessage = "File exceeds the maximum allowed size of 20 MB.";
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        if (ExcelExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            fileType = "excel";
            return true;
        }

        if (DelimitedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            fileType = "delimited";
            return true;
        }

        errorMessage = "Unsupported file extension. Allowed: .xlsx, .xls, .csv, .txt.";
        return false;
    }
}
