using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

[Authorize]
public class DestinationsController(IHttpClientFactory httpClientFactory, 
    ITokenAcquisition tokenAcquisition, 
    IConfiguration configuration) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ITokenAcquisition _tokenAcquisition = tokenAcquisition;
    private readonly IConfiguration _configuration = configuration;

    public IActionResult Index()
    { 
        return View(new List<DestinationDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchDestinations(string searchFor)
    {
        var scope = _configuration["DestinationsApi:Scopes"] ??
              throw new InvalidOperationException("Missing configuration value: DestinationsApi:Scopes");
        var accessToken = await _tokenAcquisition.GetAccessTokenForAppAsync(scope);

        var destinationsApiClient = _httpClientFactory.CreateClient("DestinationsApiClient");
        var destinations = await destinationsApiClient
            .GetFromJsonAsync<List<DestinationDto>>($"api/destinations?searchFor={searchFor}");

        return View("Index", destinations);
    }
}
