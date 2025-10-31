using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.Controllers;

public class AuthenticationController(ILogger<AuthenticationController> logger, 
    IEasyAuthProvider easyAuthProvider) : Controller
{
    private readonly ILogger<AuthenticationController> _logger = logger;
    private readonly IEasyAuthProvider _easyAuthProvider = easyAuthProvider;

    public void Login()
    {
        // TODO
    }

    public IActionResult EasyAuthData()
    {
        // for demo purposes only
        var returnValue = new
        {
            // Identity headers
            _easyAuthProvider.ClientPrincipal,
            _easyAuthProvider.ClientPrincipalId,
            _easyAuthProvider.ClientPrincipalName,
            _easyAuthProvider.ClientPrincipalIdp,
            // Helper properties
            _easyAuthProvider.IsAuthenticated,
            // Parsed client principal data
            _easyAuthProvider.RawClientPrincipalData,
            _easyAuthProvider.ClientPrincipalData,
            // Token headers
            _easyAuthProvider.IdToken,
            _easyAuthProvider.AccessToken,
            _easyAuthProvider.ExpiresOn,
            _easyAuthProvider.RefreshToken,
        };

        return Ok(returnValue);
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

    public IActionResult Logout()
    {
        // /.auth/logout?post_logout_redirect_uri=...
        var postLogoutRedirectUri = Url.Action("Index",
            "Home",
            null);

        var logoutUri = $"/.auth/logout?post_logout_redirect_uri={Uri.EscapeDataString(postLogoutRedirectUri ?? "")}";
        return Redirect(logoutUri);
    }
}
