using EnterpriseSalesPredictor.Web.Filters;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseSalesPredictor.Web.Controllers;

[Authorize]
public sealed class UploadsController : Controller
{
    private readonly UploadsApiClient _uploadsApiClient;

    public UploadsController(UploadsApiClient uploadsApiClient)
    {
        _uploadsApiClient = uploadsApiClient;
    }

    [HttpGet]
    [RequirePermission(Permissions.UploadsRead)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await BuildPageModelAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.UploadsWrite)]
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

            if (fileType == UploadPolicy.ExcelParserKey)
            {
                result = await _uploadsApiClient.UploadExcelAsync(stream, file.FileName, cancellationToken);
            }
            else
            {
                result = await _uploadsApiClient.UploadDelimitedAsync(stream, file.FileName, cancellationToken);
            }

            var viewModel = await BuildPageModelAsync(cancellationToken);
            viewModel.LastResult = result;
            viewModel.StatusMessage = "La carga fue recibida y quedó en procesamiento. Actualiza el historial para ver el resultado.";
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
    [RequirePermission(Permissions.UploadsRead)]
    public async Task<IActionResult> Errors(Guid id, CancellationToken cancellationToken)
    {
        var errors = await _uploadsApiClient.GetUploadErrorsAsync(id, cancellationToken);
        ViewData["Title"] = "Detalle de errores de carga";
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
            errorMessage = "Selecciona un archivo para cargar.";
            return false;
        }

        if (file.Length <= 0)
        {
            errorMessage = "El archivo seleccionado está vacío.";
            return false;
        }

        if (file.Length > UploadPolicy.MaxFileSizeBytes)
        {
            errorMessage = $"El archivo supera el tamaño máximo permitido de {UploadPolicy.MaxFileSizeMegabytes} MB.";
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        if (UploadPolicy.ExcelExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            fileType = UploadPolicy.ExcelParserKey;
            return true;
        }

        if (UploadPolicy.DelimitedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            fileType = UploadPolicy.DelimitedParserKey;
            return true;
        }

        errorMessage = $"Extensión de archivo no soportada. Permitidas: {UploadPolicy.AllowedExtensionsText}.";
        return false;
    }
}
