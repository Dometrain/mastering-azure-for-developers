using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.Controllers;

public class SigningSampleController(ISigningService signingService) : Controller
{
    private readonly ISigningService _signingService = signingService;

    public IActionResult Sign(string textToSign)
    {
        var signature = _signingService.SignData(textToSign);
        // escape the signature before sending it back - like that, we can ensure
        // that, when we send it to the VerifySignature action via a query string value, the correct
        // characters remain.  There's no need to do this when not transmitting the text like that.
        return Content(Uri.EscapeDataString(signature));
    }

    public IActionResult VerifySignature(string textThatWasSigned, string signature)
    {
        // no need to do anything with the escaped signature, ASP.NET Core automatically unescapes it
        return Content(_signingService.VerifySignature(textThatWasSigned, signature).ToString());
    }
}

