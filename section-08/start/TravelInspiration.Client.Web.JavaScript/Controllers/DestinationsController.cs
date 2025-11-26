using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelInspiration.Client.Web.Models.Dto;

namespace TravelInspiration.Client.Web.Controllers;

[Authorize]
public class DestinationsController : Controller
{
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

    public IActionResult Index()
    { 
        return View(new List<DestinationDto>());
    }

    [HttpGet]
    public async Task<IActionResult> SearchDestinations(string searchFor)
    {
        // this is a local API endpoint 
        var filteredDestinations = _destinations
            .Where(d => d.Name.Contains(searchFor ?? "")).ToList();
         
        return Json(filteredDestinations);
    }
}
