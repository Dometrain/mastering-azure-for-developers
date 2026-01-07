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

    [HttpGet]
    public async Task<IActionResult> SearchItineraries(string searchFor)
    {
        // proxy to remote API - the access token is auto-attached
        var itinerariesApiClient = _httpClientFactory.CreateClient("ItinerariesApiClient");
        var itineraries = await itinerariesApiClient
            .GetFromJsonAsync<List<ItineraryDto>>($"itineraries-api/api/itineraries?searchFor={searchFor}");

        return Json(itineraries);
    }
}