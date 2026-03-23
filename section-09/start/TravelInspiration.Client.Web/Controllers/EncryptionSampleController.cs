using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.Controllers;

public class EncryptionSampleController(IEncryptionService encryptionService) : Controller
{
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task<IActionResult> Encrypt(string plainText)
    {
        var encryptedText = await _encryptionService.EncryptAsync(plainText);
        // escape the encrypted text before sending it back - like that, we can ensure
        // that, when we send it to the decrypt action via a query string value, the correct
        // characters remain.  There's no need to do this when not transmitting the text like that.
        return Content(Uri.EscapeDataString(encryptedText));
    }

    public async Task<IActionResult> Decrypt(string encryptedText)
    {
        // no need to do anything with the encrypted text, ASP.NET Core automatically unescapes it
        return Content(await _encryptionService.DecryptAsync(encryptedText));
    }
}
