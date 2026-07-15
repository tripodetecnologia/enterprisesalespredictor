namespace EnterpriseSalesPredictor.Application.DTOs.Dashboard;

public sealed class DashboardAlertDto
{
    public string Severity { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
