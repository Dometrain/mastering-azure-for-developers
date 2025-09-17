using Duende.AccessTokenManagement;
using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

public class ItinerariesController(IHttpClientFactory httpClientFactory,
    IClientCredentialsTokenManager clientCredentialsTokenManager) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IClientCredentialsTokenManager _clientCredentialsTokenManager = clientCredentialsTokenManager;

    public IActionResult Index()
    {      
        return View(new List<ItineraryDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchItineraries(string searchFor)
    {
        // load the token to check it out
        var accessToken = await _clientCredentialsTokenManager
            .GetAccessTokenAsync(ClientCredentialsClientName.Parse("ItinerariesClientCredentialsFlow"))
            .GetToken();

        var itinerariesApiClient = _httpClientFactory.CreateClient("ItinerariesApiClient");
        var itineraries = await itinerariesApiClient
            .GetFromJsonAsync<List<ItineraryDto>>($"api/itineraries?searchFor={searchFor}");

        return View("Index", itineraries);
    }
}
