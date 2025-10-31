using System.Text.Json.Serialization;

namespace TravelInspiration.Client.Web.Services;

public class EasyAuthClaim
{
    [JsonPropertyName("typ")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("val")]
    public string Value { get; set; } = string.Empty;

}
