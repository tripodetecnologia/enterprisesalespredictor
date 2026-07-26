namespace EnterpriseSalesPredictor.Infrastructure.FileProcessing;

public sealed class UploadStorageOptions
{
    public const string SectionName = "Uploads:Storage";

    public string RootPath { get; set; } = Path.Combine("App_Data", "uploads");
}
