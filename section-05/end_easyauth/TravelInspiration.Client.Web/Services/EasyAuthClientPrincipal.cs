using System.Text.Json.Serialization;

namespace TravelInspiration.Client.Web.Services;

public class EasyAuthClientPrincipal
{
    [JsonPropertyName("auth_typ")]
    public string? AuthenticationType { get; set; }

    [JsonPropertyName("claims")]
    public EasyAuthClaim[]? Claims { get; set; }

    [JsonPropertyName("name_typ")]
    public string? NameClaimType { get; set; }

    [JsonPropertyName("role_typ")]
    public string? RoleClaimType { get; set; }

}
