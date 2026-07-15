namespace EnterpriseSalesPredictor.Application.Interfaces.Uploads;

public sealed class UploadParseError
{
    public int RowNumber { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}
