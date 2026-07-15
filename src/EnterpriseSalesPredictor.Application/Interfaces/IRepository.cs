namespace EnterpriseSalesPredictor.Application.Interfaces;

public interface IRepository<TEntity> : IReadRepository<TEntity>
    where TEntity : class
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}
