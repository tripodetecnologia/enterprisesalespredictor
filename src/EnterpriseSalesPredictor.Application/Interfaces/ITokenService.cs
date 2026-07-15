namespace EnterpriseSalesPredictor.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(AuthenticatedUser user);
}
