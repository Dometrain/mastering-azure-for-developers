using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

public class ItinerariesController(IHttpClientFactory httpClientFactory) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    public IActionResult Index()
    {      
        return View(new List<ItineraryDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchItineraries(string searchFor)
    {
        var itinerariesApiClient = _httpClientFactory.CreateClient("ItinerariesApiClient");
        var itineraries = await itinerariesApiClient
            .GetFromJsonAsync<List<ItineraryDto>>($"api/itineraries?searchFor={searchFor}");

        return View("Index", itineraries);
    }
}
