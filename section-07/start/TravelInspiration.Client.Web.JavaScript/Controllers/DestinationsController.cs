using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

public class DestinationsController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public IActionResult Index()
    { 
        return View(new List<DestinationDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchDestinations(string searchFor)
    {
        var destinationsApiClient = _httpClientFactory.CreateClient("DestinationsApiClient");
        var destinations = await destinationsApiClient
            .GetFromJsonAsync<List<DestinationDto>>($"api/destinations?searchFor={searchFor}");

        return View("Index", destinations);
    }
}
