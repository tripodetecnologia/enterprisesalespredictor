namespace EnterpriseSalesPredictor.Application.Interfaces.Exports;

public sealed class ExportFileDto
{
    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public byte[] Content { get; set; } = Array.Empty<byte>();
}
