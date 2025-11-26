using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

[Authorize]
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
        //var tokenResponse = await HttpContext.GetUserAccessTokenAsync(
        //    new UserTokenRequestParameters
        //    {
        //        ForceTokenRenewal = true
        //    });

        var itinerariesApiClient = _httpClientFactory.CreateClient("ItinerariesApiClient"); 
        var itineraries = await itinerariesApiClient
            .GetFromJsonAsync<List<ItineraryDto>>($"itineraries-api/api/itineraries?searchFor={searchFor}");

        return View("Index", itineraries);
    }
}
