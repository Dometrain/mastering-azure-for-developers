using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using TravelInspiration.Client.Web.Models.Dto;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.Controllers;

public class ItinerariesController(IHttpClientFactory httpClientFactory,
     ITokenAcquisition tokenAcquisition,
     IConfiguration configuration,
     IEasyAuthProvider easyAuthProvider) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ITokenAcquisition _tokenAcquisition = tokenAcquisition;
    private readonly IConfiguration _configuration = configuration;
    private readonly IEasyAuthProvider _easyAuthProvider = easyAuthProvider;

    public IActionResult Index()
    {
        return View(new List<ItineraryDto>());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchItineraries(string searchFor)
    {
        // Acquire access token for debugging/inspection    
        //var scope = _configuration["ItinerariesApi:Scopes"] ??
        //        throw new InvalidOperationException("Missing configuration value: ItinerariesApi:Scopes");
        //var accessToken = await _tokenAcquisition.GetAccessTokenForAppAsync(scope);

        //var itinerariesApiClient = _httpClientFactory.CreateClient("ItinerariesApiClient");

        var itinerariesApiClient = _httpClientFactory.CreateClient("UserBasedItinerariesApiClient");
        itinerariesApiClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _easyAuthProvider.AccessToken ?? "");
        var itineraries = await itinerariesApiClient
            .GetFromJsonAsync<List<ItineraryDto>>($"api/itineraries?searchFor={searchFor}");
        return View("Index", itineraries);
    }
 }