namespace EnterpriseSalesPredictor.Application.Interfaces;

public interface ICredentialValidator
{
    Task<AuthenticatedUser?> ValidateAsync(string username, string password, CancellationToken cancellationToken = default);
}
