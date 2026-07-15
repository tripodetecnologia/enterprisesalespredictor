namespace EnterpriseSalesPredictor.Application.DTOs.Reports;

public sealed class ReportSectionDto
{
    public string Title { get; set; } = string.Empty;

    public IReadOnlyCollection<ReportMetricDto> Metrics { get; set; } = Array.Empty<ReportMetricDto>();
}
