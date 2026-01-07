using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelInspiration.Client.Web.Simplified.Controllers;

[Authorize]
public class ProtectedController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
