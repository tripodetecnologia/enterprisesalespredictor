namespace EnterpriseSalesPredictor.Application.Constants;

public static class UploadPolicy
{
    public const int BytesPerMegabyte = 1024 * 1024;
    public const int MaxFileSizeMegabytes = 20;
    public const long MaxFileSizeBytes = MaxFileSizeMegabytes * BytesPerMegabyte;
    public const string ExcelParserKey = "excel";
    public const string DelimitedParserKey = "delimited";

    public static readonly string[] ExcelExtensions = { ".xlsx", ".xls" };
    public static readonly string[] DelimitedExtensions = { ".csv", ".txt" };
    public static readonly string[] AllExtensions = ExcelExtensions.Concat(DelimitedExtensions).ToArray();

    public static string AcceptAttribute => string.Join(',', AllExtensions);
    public static string AllowedExtensionsText => string.Join(", ", AllExtensions);
}
