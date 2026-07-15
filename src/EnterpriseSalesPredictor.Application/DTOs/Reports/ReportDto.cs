namespace EnterpriseSalesPredictor.Application.DTOs.Reports;

public sealed class ReportDto
{
    public string Title { get; set; } = string.Empty;

    public DateTime GeneratedAtUtc { get; set; }

    public IReadOnlyCollection<ReportSectionDto> Sections { get; set; } = Array.Empty<ReportSectionDto>();
}
