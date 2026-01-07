using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TravelInspiration.API.TokenEnrichment;

public class TokenEnrichmentFunction
{
    private readonly ILogger<TokenEnrichmentFunction> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = 
        new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public TokenEnrichmentFunction(ILogger<TokenEnrichmentFunction> logger)
    {
        _logger = logger;
    }

    [Function("EnrichToken")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var eventData = JsonSerializer.Deserialize<TokenIssuanceStartEventData>(requestBody,
             _jsonSerializerOptions);

        if (eventData?.Data?.AuthenticationContext?.User?.Mail == null)
        {
            return new BadRequestObjectResult("Invalid event data");
        }

        var userEmail = eventData.Data.AuthenticationContext.User.Mail;
        var customerTier = await GetCustomerTierAsync(userEmail);

        var claims = new Dictionary<string, object>
         {
             { "customer_tier", customerTier }
         };

        var response = new TokenIssuanceStartResponse
        {
            Data = new TokenIssuanceStartResponseData
            {
                Actions =
                [
                    new()
                         {
                             Claims = claims
                         }
                ]
            }
        };

        return new OkObjectResult(response);
    }

    private async Task<string> GetCustomerTierAsync(string email)
    {
        if (email.ToLower().EndsWith("sharklasers.com"))
        {
            return "Premium";
        }
        return "Free";
    }
}

public class TokenIssuanceStartEventData
{
    public TokenIssuanceStartData? Data { get; set; }
}

public class TokenIssuanceStartData
{
    public AuthenticationContext? AuthenticationContext { get; set; }
}

public class AuthenticationContext
{
    public User? User { get; set; }
}

public class User
{
    public string? Mail { get; set; }
}

public class TokenIssuanceStartResponse
{
    public TokenIssuanceStartResponseData Data { get; set; } = new();
}

public class TokenIssuanceStartResponseData
{
    [JsonPropertyName("@odata.type")]
    public string Type { get; set; } = "microsoft.graph.onTokenIssuanceStartResponseData";
    public List<TokenIssuanceStartAction> Actions { get; set; } = [];
}

public class TokenIssuanceStartAction
{
    [JsonPropertyName("@odata.type")]
    public string Type { get; set; } = "microsoft.graph.tokenIssuanceStart.provideClaimsForToken";
    public Dictionary<string, object> Claims { get; set; } = [];
}