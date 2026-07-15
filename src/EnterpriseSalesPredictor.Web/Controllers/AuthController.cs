using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EnterpriseSalesPredictor.Web.Configuration;
using EnterpriseSalesPredictor.Web.Services;
using EnterpriseSalesPredictor.Web.ViewModels.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EnterpriseSalesPredictor.Web.Controllers;

public sealed class AuthController : Controller
{
    private readonly IWebAuthApiClient _webAuthApiClient;
    private readonly IOptionsMonitor<JwtClientOptions> _jwtClientOptions;

    public AuthController(IWebAuthApiClient webAuthApiClient, IOptionsMonitor<JwtClientOptions> jwtClientOptions)
    {
        _webAuthApiClient = webAuthApiClient;
        _jwtClientOptions = jwtClientOptions;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl ?? string.Empty
        });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginResult = await _webAuthApiClient.LoginAsync(model.Username, model.Password, cancellationToken);
        if (!loginResult.IsSuccess)
        {
            model.ErrorMessage = loginResult.Error;
            return View(model);
        }

        var principal = BuildClaimsPrincipal(loginResult.AccessToken);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }

    private ClaimsPrincipal BuildClaimsPrincipal(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var options = _jwtClientOptions.CurrentValue;

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.Issuer,
            ValidAudience = options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        var principal = handler.ValidateToken(accessToken, tokenValidationParameters, out _);
        var claims = principal.Claims.ToList();
        claims.Add(new Claim("access_token", accessToken));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        return new ClaimsPrincipal(claimsIdentity);
    }
}
