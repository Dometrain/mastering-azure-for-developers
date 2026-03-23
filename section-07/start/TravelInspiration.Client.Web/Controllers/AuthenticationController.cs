using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelInspiration.Client.Web.Controllers;

public class AuthenticationController(ILogger<AuthenticationController> logger) : Controller
{
    private readonly ILogger<AuthenticationController> _logger = logger;

    public async Task Login()
    {
        // challenge the OIDC scheme to log in, and redirect to the app root afterwards
        await HttpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme,
            new AuthenticationProperties()
            {
                RedirectUri = "/"
            });
    }

    [Authorize]
    public async Task<IActionResult> Tokens()
    {    
        // for demo purposes only, don't expose tokens like this in production code!
        var returnValue =
            new
            {
                IdentityToken = await HttpContext.GetTokenAsync("id_token"),
                AcessToken = await HttpContext.GetTokenAsync("access_token"),
                RefreshToken = await HttpContext.GetTokenAsync("refresh_token"),
            };
        return Ok(returnValue);
    }

    [Authorize]
    public async Task Logout()
    {
        // Clears the  local cookie
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirects to the IDP linked to scheme
        // "OpenIdConnectDefaults.AuthenticationScheme" (oidc)
        // so it can clear its own session/cookie
        await HttpContext.SignOutAsync(
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
