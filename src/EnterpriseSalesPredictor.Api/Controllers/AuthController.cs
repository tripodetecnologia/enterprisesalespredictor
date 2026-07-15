using EnterpriseSalesPredictor.Api.Contracts.Auth;
using EnterpriseSalesPredictor.Application.Interfaces;
using EnterpriseSalesPredictor.Application.Validators;
using EnterpriseSalesPredictor.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EnterpriseSalesPredictor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ICredentialValidator _credentialValidator;
    private readonly ITokenService _tokenService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IOptionsMonitor<JwtOptions> _jwtOptions;

    public AuthController(
        ICredentialValidator credentialValidator,
        ITokenService tokenService,
        IAuthorizationService authorizationService,
        IOptionsMonitor<JwtOptions> jwtOptions)
    {
        _credentialValidator = credentialValidator;
        _tokenService = tokenService;
        _authorizationService = authorizationService;
        _jwtOptions = jwtOptions;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        Guard.AgainstNullOrWhiteSpace(request.Username, nameof(request.Username));
        Guard.AgainstNullOrWhiteSpace(request.Password, nameof(request.Password));

        var authenticatedUser = await _credentialValidator.ValidateAsync(request.Username, request.Password, cancellationToken);
        if (authenticatedUser is null)
        {
            return Unauthorized();
        }

        return Ok(new LoginResponse
        {
            AccessToken = _tokenService.GenerateToken(authenticatedUser),
            ExpiresInMinutes = _jwtOptions.CurrentValue.ExpirationMinutes
        });
    }

    [HttpGet("permissions")]
    [Authorize]
    public IActionResult Permissions()
    {
        var permissions = User.FindAll("permission").Select(claim => claim.Value);
        return Ok(new
        {
            user = User.Identity?.Name,
            permissions
        });
    }

    [HttpPost("authorize")]
    [Authorize]
    public async Task<IActionResult> AuthorizeActionAsync([FromBody] AuthorizationCheckRequest request)
    {
        Guard.AgainstNullOrWhiteSpace(request.Module, nameof(request.Module));
        Guard.AgainstNullOrWhiteSpace(request.Action, nameof(request.Action));

        var policy = $"Permission:{request.Module}:{request.Action}";
        var result = await _authorizationService.AuthorizeAsync(User, policy);

        if (result.Succeeded)
        {
            return Ok(new { allowed = true, policy });
        }

        return Forbid();
    }
}
