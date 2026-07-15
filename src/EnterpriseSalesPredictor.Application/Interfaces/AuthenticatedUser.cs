namespace EnterpriseSalesPredictor.Application.Interfaces;

public sealed record AuthenticatedUser(
    string UserId,
    string Username,
    string Role,
    IReadOnlyCollection<string> Permissions);
