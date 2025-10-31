using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

public class DestinationsController(IHttpClientFactory httpClientFactory, 
    IConfiguration configuration) : Controller
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;

    public IActionResult Index()
    { 
        return View(new List<DestinationDto>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SearchDestinations(string searchFor)
    {
        //var entraIdClient = _httpClientFactory.CreateClient("EntraIdClient");
        //var discoveryResponse = await entraIdClient
        //    .GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest()
        //    {
        //        Address = _configuration["EntraIdConfiguration:Authority"] ??
        //            throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:Authority"),
        //        Policy = new DiscoveryPolicy()
        //        {
        //            ValidateEndpoints = false
        //        }
        //    });

        //if (discoveryResponse.IsError)
        //{
        //    throw new Exception(discoveryResponse.Error);
        //}

        //var tokenResponse = await entraIdClient.RequestClientCredentialsTokenAsync(
        //    new ClientCredentialsTokenRequest()
        //    {
        //        Address = discoveryResponse.TokenEndpoint,
        //        ClientId = _configuration["EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientId"] ??
        //            throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientId"),
        //        ClientSecret = _configuration["EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientSecret"] ??
        //            throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientSecret"),
        //        Scope = configuration["EntraIdConfiguration:DestinationsClientCredentialsFlow:Scope"] ??
        //            throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:DestinationsClientCredentialsFlow:Scope")
        //    });

        //if (tokenResponse.IsError)
        //{
        //    throw new Exception(tokenResponse.Error);
        //}

        var destinationsApiClient = _httpClientFactory.CreateClient("DestinationsApiClient");
        // destinationsApiClient.SetBearerToken(tokenResponse.AccessToken ?? "");
        var destinations = await destinationsApiClient
            .GetFromJsonAsync<List<DestinationDto>>($"api/destinations?searchFor={searchFor}");

        return View("Index", destinations);
    }
}
