using EnterpriseSalesPredictor.Application.Interfaces.Auditing;
using EnterpriseSalesPredictor.Domain.Entities;
using EnterpriseSalesPredictor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseSalesPredictor.Infrastructure.Auditing;

public sealed class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _dbContext;

    public AuditLogService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AuditLogDto> RecordAsync(CreateAuditLogCommand command, CancellationToken cancellationToken = default)
    {
        var entry = new AuditLog(
            Guid.NewGuid(),
            DateTime.UtcNow,
            command.Actor,
            command.Action,
            command.Module,
            command.Details);

        await _dbContext.AuditLogs.AddAsync(entry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(entry);
    }

    public async Task<IReadOnlyCollection<AuditLogDto>> GetAuditLogsAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _dbContext.AuditLogs
            .AsNoTracking()
            .OrderByDescending(entry => entry.OccurredAtUtc)
            .ToListAsync(cancellationToken);

        return entries.Select(Map).ToArray();
    }

    private static AuditLogDto Map(AuditLog entity)
    {
        return new AuditLogDto
        {
            Id = entity.Id,
            OccurredAtUtc = entity.OccurredAtUtc,
            Actor = entity.Actor,
            Action = entity.Action,
            Module = entity.Module,
            Details = entity.Details
        };
    }
}
