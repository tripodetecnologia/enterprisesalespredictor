namespace EnterpriseSalesPredictor.Web.Configuration;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public string BaseUrl { get; set; } = "https://localhost:7042";
}
