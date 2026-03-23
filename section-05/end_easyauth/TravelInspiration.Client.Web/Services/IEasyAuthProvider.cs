namespace TravelInspiration.Client.Web.Services;

public interface IEasyAuthProvider
{
    string? ClientPrincipal { get; }
    string? ClientPrincipalId { get; }
    string? ClientPrincipalIdp { get; }
    string? ClientPrincipalName { get; }

    // Token-related headers
    string? IdToken { get; }
    string? AccessToken { get; }
    string? ExpiresOn { get; }
    string? RefreshToken { get; }
    bool IsAuthenticated { get; }

    public string? RawClientPrincipalData { get; }
    EasyAuthClientPrincipal? ClientPrincipalData { get; }
}