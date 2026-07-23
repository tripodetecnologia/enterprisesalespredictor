namespace EnterpriseSalesPredictor.Web.ViewModels.Audit;

public sealed class AuditPageViewModel
{
    public AuditFilterViewModel Filters { get; set; } = new();

    public PagedAuditSectionViewModel UploadLogs { get; set; } = new();

    public PagedAuditSectionViewModel ExportLogs { get; set; } = new();

    public PagedAuditSectionViewModel FunctionalLogs { get; set; } = new();
}

public sealed class PagedAuditSectionViewModel
{
    public IReadOnlyCollection<AuditLogItemViewModel> Items { get; set; } = Array.Empty<AuditLogItemViewModel>();

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages { get; set; }
}
