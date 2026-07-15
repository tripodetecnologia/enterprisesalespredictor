namespace EnterpriseSalesPredictor.Application.Interfaces.Auditing;

public sealed class CreateAuditLogCommand
{
    public string Actor { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
