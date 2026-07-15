namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class UploadErrorDto
{
    public Guid Id { get; set; }

    public Guid UploadId { get; set; }

    public int RowNumber { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}
