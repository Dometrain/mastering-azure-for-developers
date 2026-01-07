using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.Controllers;

public class SigningSampleController(ISigningService signingService) : Controller
{
    private readonly ISigningService _signingService = signingService;

    public IActionResult Sign(string textToSign)
    {
        var signature = _signingService.SignData(textToSign);
        return Content(Uri.EscapeDataString(signature));
    }

    public IActionResult VerifySignature(string textThatWasSigned, string signature)
    {
        return Content(_signingService.VerifySignature(textThatWasSigned, signature).ToString());
    }
}
