namespace EnterpriseSalesPredictor.Application.Interfaces;

public interface IReadRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);
}
