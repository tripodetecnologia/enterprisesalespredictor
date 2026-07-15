namespace EnterpriseSalesPredictor.Web.ViewModels.Reports;

public sealed class ReportSectionViewModel
{
    public string Title { get; set; } = string.Empty;

    public IReadOnlyCollection<ReportMetricViewModel> Metrics { get; set; } = Array.Empty<ReportMetricViewModel>();
}
