namespace EnterpriseSalesPredictor.Web.ViewModels.Audit;

public sealed class AuditPageViewModel
{
    public AuditFilterViewModel Filters { get; set; } = new();

    public IReadOnlyCollection<AuditLogItemViewModel> UploadLogs { get; set; } = Array.Empty<AuditLogItemViewModel>();

    public IReadOnlyCollection<AuditLogItemViewModel> ExportLogs { get; set; } = Array.Empty<AuditLogItemViewModel>();

    public IReadOnlyCollection<AuditLogItemViewModel> FunctionalLogs { get; set; } = Array.Empty<AuditLogItemViewModel>();
}
