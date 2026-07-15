namespace EnterpriseSalesPredictor.Infrastructure.Security;

public interface ISecurityBootstrapper
{
    Task EnsureSeededAsync(CancellationToken cancellationToken = default);
}
