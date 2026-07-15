namespace EnterpriseSalesPredictor.Web.ViewModels.Reports;

public sealed class ReportViewModel
{
    public string Title { get; set; } = string.Empty;

    public DateTime GeneratedAtUtc { get; set; }

    public IReadOnlyCollection<ReportSectionViewModel> Sections { get; set; } = Array.Empty<ReportSectionViewModel>();
}
