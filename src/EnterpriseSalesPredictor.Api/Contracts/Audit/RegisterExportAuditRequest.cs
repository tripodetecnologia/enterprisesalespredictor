namespace EnterpriseSalesPredictor.Api.Contracts.Audit;

public sealed class RegisterExportAuditRequest
{
    public string ExportType { get; set; } = string.Empty;

    public string Filters { get; set; } = string.Empty;
}
