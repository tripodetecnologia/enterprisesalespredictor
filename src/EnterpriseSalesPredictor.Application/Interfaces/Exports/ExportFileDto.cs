using EnterpriseSalesPredictor.Application.Constants;

namespace EnterpriseSalesPredictor.Application.Interfaces.Exports;

public sealed class ExportFileDto
{
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = ExportFormats.ExcelContentType;

    public byte[] Content { get; set; } = Array.Empty<byte>();
}
