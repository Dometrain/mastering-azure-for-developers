using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

[Authorize]
public class ItinerariesController(IHttpClientFactory httpClientFactory,
     ITokenAcquisition tokenAcquisition,
     IConfiguration configuration) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ITokenAcquisition _tokenAcquisition = tokenAcquisition;
    private readonly IConfiguration _configuration = configuration;

    public IActionResult Index()
    {
        return View(new List<ItineraryDto>());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchItineraries(string searchFor)
    {
        // Acquire access token for debugging/inspection    
        var scopes = _configuration.GetSection("ItinerariesApi:UserBasedScopes").Get<string[]>() ??
            throw new InvalidOperationException("Missing configuration value: ItinerariesApi:UserBasedScopes");
        var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);

        var itinerariesApiClient = _httpClientFactory.CreateClient("ItinerariesApiClient");
        var itineraries = await itinerariesApiClient
            .GetFromJsonAsync<List<ItineraryDto>>($"itineraries-api/api/itineraries?searchFor={searchFor}");
        return View("Index", itineraries);
    }
 }