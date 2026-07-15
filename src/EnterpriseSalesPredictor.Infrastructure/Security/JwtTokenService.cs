using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EnterpriseSalesPredictor.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseSalesPredictor.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;

    public JwtTokenService(IOptionsMonitor<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    public string GenerateToken(AuthenticatedUser user)
    {
        var options = _jwtOptions.CurrentValue;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(ClaimTypes.NameIdentifier, user.UserId),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        claims.AddRange(user.Permissions.Select(permission => new Claim("permission", permission)));

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
