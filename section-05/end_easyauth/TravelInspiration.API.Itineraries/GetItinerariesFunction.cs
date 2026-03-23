using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TravelInspiration.API.Itineraries.DbContexts;
using TravelInspiration.API.Itineraries.Models;

namespace TravelInspiration.API.Itineraries;

public class GetItinerariesFunction(ILogger<GetItinerariesFunction> logger,
    TravelInspirationDbContext dbContext,
    IConfiguration configuration)
{
    private readonly ILogger<GetItinerariesFunction> _logger = logger;
    private readonly TravelInspirationDbContext _dbContext = dbContext;
    private readonly IConfiguration _configuration = configuration;

    [Function("GetItinerariesFunction")]
    [OpenApiOperation("GetItineraries", 
        "GetItineraries", 
        Description = "Get the itineraries.")]
    [OpenApiParameter("SearchFor", 
        In = Microsoft.OpenApi.Models.ParameterLocation.Query, 
        Required = false, 
        Type = typeof(string), 
        Description = "Search for itineraries by part of their name or description.")]
    [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, 
        "application/json",
        typeof(List<ItineraryDto>))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "itineraries")] 
        HttpRequest req)
    {
        string? userOid = GetUserOidFromEasyAuth(req);
        if (string.IsNullOrEmpty(userOid))
        {
            _logger.LogWarning("No user object identifier found in Easy Auth headers");
            // return unauthorized result, as we don't want to unintenionally return 
            // data that doesn't belong to the current user
            return new UnauthorizedResult();
        }

        string? searchForValue = req.Query["SearchFor"];

        var itineraryEntities = await _dbContext.Itineraries
            .Where(i => i.UserId == userOid &&
                        (searchForValue == null ||
                           i.Name.Contains(searchForValue) ||
                           (i.Description != null && i.Description.Contains(searchForValue))))
            .OrderBy(i => i.Name)
            .ToListAsync();

        var itineraryDtos = itineraryEntities.Select(i => new ItineraryDto()
        {
            Id = i.Id,
            Description = i.Description,
            Name = i.Name,
            UserId = i.UserId
        });

        return new OkObjectResult(itineraryDtos);
    }

    private string? GetUserOidFromEasyAuth(HttpRequest request)
    {
        if (request.Headers.TryGetValue("X-MS-CLIENT-PRINCIPAL-ID", out var principalIdHeader))
        {
            return principalIdHeader.FirstOrDefault();
        }
        return null;
    }
}