using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Models.Dto;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.Controllers;

public class ItinerariesController(IHttpClientFactory httpClientFactory,
    IEasyAuthProvider easyAuthProvider) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IEasyAuthProvider _easyAuthProvider = easyAuthProvider;

    public IActionResult Index()
    {      
        return View(new List<ItineraryDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchItineraries(string searchFor)
    {
        // load the token to check it out
 
        var itinerariesApiClient = _httpClientFactory.CreateClient("UserBasedItinerariesApiClient");
        itinerariesApiClient.SetBearerToken(_easyAuthProvider.AccessToken ?? "");
        var itineraries = await itinerariesApiClient
            .GetFromJsonAsync<List<ItineraryDto>>($"api/itineraries?searchFor={searchFor}");

        return View("Index", itineraries);
    }
}
