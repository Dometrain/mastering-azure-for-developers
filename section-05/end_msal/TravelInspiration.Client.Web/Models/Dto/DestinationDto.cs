namespace TravelInspiration.Client.Web.Models.Dto;

public sealed class DestinationDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public List<string> ImageUris { get; set; } = [];
}
