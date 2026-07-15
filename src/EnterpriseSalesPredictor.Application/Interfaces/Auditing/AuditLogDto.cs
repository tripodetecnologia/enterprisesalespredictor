namespace EnterpriseSalesPredictor.Application.Interfaces.Auditing;

public sealed class AuditLogDto
{
    public Guid Id { get; set; }

    public DateTime OccurredAtUtc { get; set; }

    public string Actor { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string Module { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;
}
