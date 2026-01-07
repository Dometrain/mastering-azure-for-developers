using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.Controllers;

public class EncryptionSampleController(IEncryptionService encryptionService) : Controller
{
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task<IActionResult> Encrypt(string plainText)
    {
        var encryptedText = await _encryptionService.EncryptAsync(plainText);
        return Content(Uri.EscapeDataString(encryptedText));
    }

    public async Task<IActionResult> Decrypt(string encryptedText)
    {
        return Content(await _encryptionService.DecryptAsync(encryptedText));
    }
}
