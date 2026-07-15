namespace EnterpriseSalesPredictor.Web.ViewModels.Reports;

public sealed class ReportsPageViewModel
{
    public ReportFiltersViewModel Filters { get; set; } = new();

    public ReportViewModel ManagementReport { get; set; } = new();

    public ReportViewModel CommercialReport { get; set; } = new();

    public ReportViewModel OperationalReport { get; set; } = new();

    public ReportViewModel ReplenishmentReport { get; set; } = new();

    public ReportViewModel PredictiveReport { get; set; } = new();
}
