namespace EnterpriseSalesPredictor.Domain.Entities;

public sealed class AuditLog : Entity
{
    public AuditLog(
        Guid id,
        DateTime occurredAtUtc,
        string actor,
        string action,
        string module,
        string details)
        : base(id)
    {
        OccurredAtUtc = occurredAtUtc;
        Actor = actor;
        Action = action;
        Module = module;
        Details = details;
    }

    public DateTime OccurredAtUtc { get; private set; }

    public string Actor { get; private set; }

    public string Action { get; private set; }

    public string Module { get; private set; }

    public string Details { get; private set; }
}
