using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelInspiration.Client.Web.Simplified.Controllers;

public class AuthenticationController() : Controller
{
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
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}