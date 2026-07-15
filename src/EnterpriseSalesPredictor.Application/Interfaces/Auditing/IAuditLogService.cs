namespace EnterpriseSalesPredictor.Application.Interfaces.Auditing;

public interface IAuditLogService
{
    Task<AuditLogDto> RecordAsync(CreateAuditLogCommand command, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuditLogDto>> GetAuditLogsAsync(CancellationToken cancellationToken = default);
}
