using Duende.AccessTokenManagement;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace TravelInspiration.Client.Web.ConfigureOptions;

public class ClientCredentialsClientConfigureOptions(DiscoveryCache discoveryCache,
    IConfiguration configuration) : IConfigureNamedOptions<ClientCredentialsClient>
{
    private readonly DiscoveryCache _discoveryCache = discoveryCache;
    private readonly IConfiguration _configuration = configuration;

    public void Configure(string? name, ClientCredentialsClient options)
    {
        var discoveryResponse = _discoveryCache.GetAsync().GetAwaiter().GetResult();

        if (discoveryResponse.IsError)
        {
            throw new Exception(discoveryResponse.Error);
        }

        if (name == "DestinationsClientCredentialsFlow")
        {
            options.TokenEndpoint = new Uri(discoveryResponse.TokenEndpoint ??
                throw new InvalidOperationException("TokenEndpoint is null"));
            options.ClientId = ClientId.Parse(_configuration["EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientId"] ??
               throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientId"));
            options.ClientSecret = ClientSecret.Parse(_configuration["EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientSecret"] ??
                throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:DestinationsClientCredentialsFlow:ClientSecret"));
            options.Scope = Scope.Parse(_configuration["EntraIdConfiguration:DestinationsClientCredentialsFlow:Scope"] ??
                throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:DestinationsClientCredentialsFlow:Scope"));

        }
        else if (name == "ItinerariesClientCredentialsFlow")
        {
            options.TokenEndpoint = new Uri(discoveryResponse.TokenEndpoint ??
                throw new InvalidOperationException("TokenEndpoint is null"));
            options.ClientId = ClientId.Parse(_configuration["EntraIdConfiguration:ItinerariesClientCredentialsFlow:ClientId"] ??
                throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:ItinerariesClientCredentialsFlow:ClientId"));
            options.ClientSecret = ClientSecret.Parse(_configuration["EntraIdConfiguration:ItinerariesClientCredentialsFlow:ClientSecret"] ??
                throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:ItinerariesClientCredentialsFlow:ClientSecret"));
            options.Scope = Scope.Parse(_configuration["EntraIdConfiguration:ItinerariesClientCredentialsFlow:Scope"] ??
                throw new InvalidOperationException("Missing configuration value EntraIdConfiguration:ItinerariesClientCredentialsFlow:Scope"));

        }
    }

    public void Configure(ClientCredentialsClient options)
    {
        throw new NotImplementedException();
    }
}
