using MediatR;
using System.Security.Claims;
using TravelInspiration.API.Destinations.Shared.Slices;

namespace TravelInspiration.API.Destinations.Features.Destinations;

public sealed class SearchDestinations : ISlice
{
    public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapGet("api/destinations",
             (string? searchFor,
                IMediator mediator,
                CancellationToken cancellationToken,
                ClaimsPrincipal user) =>
             {
                 var userClaims = user.Claims;
                 return mediator.Send(
                     new SearchDestinationsQuery(searchFor),
                     cancellationToken);
             }).RequireAuthorization("DestinationsReadRoleIsRequired"); 
    }

    public sealed class SearchDestinationsQuery(string? searchFor) : IRequest<IResult>
    {
        public string? SearchFor { get; } = searchFor;
    }

    public sealed class DestinationDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public List<string> ImageUris { get; set; } = [];

    }

    public sealed class SearchDestinationsHandler(IConfiguration configuration) :
       IRequestHandler<SearchDestinationsQuery, IResult>
    {
        private readonly IConfiguration _configuration = configuration;

        // for demo purposes, create a list with a few dummy destinations
        private readonly List<DestinationDto> _destinations =
        [
            new () { Id = 1, Name = "Paris, France" },
            new () { Id = 2, Name = "Tokyo, Japan" },
            new () { Id = 3, Name = "New York City, USA" },
            new () { Id = 4, Name = "London, England" },
            new () { Id = 5, Name = "Rome, Italy" },
            new () { Id = 6, Name = "Sydney, Australia" },
            new () { Id = 7, Name = "Barcelona, Spain" },
            new () { Id = 8, Name = "Amsterdam, Netherlands" },
            new () { Id = 9, Name = "Bangkok, Thailand" },
            new () { Id = 10, Name = "Dubai, UAE" },
            new () { Id = 11, Name = "Santorini, Greece" },
            new () { Id = 12, Name = "Bali, Indonesia" }
        ];

        public Task<IResult> Handle(SearchDestinationsQuery request,
            CancellationToken cancellationToken)
        {
            // filter the list of dummy destinations
            var filteredDestinations = _destinations
                .Where(d => d.Name.Contains(request.SearchFor ?? ""));

            // return results, returning only the amount described in settings
            return Task.FromResult(Results.Ok(filteredDestinations));
        }
    }
}
