using System.Text.Json;

namespace TravelInspiration.Client.Web.Services;

public class EasyAuthProvider(IHttpContextAccessor httpContextAccessor,
    ILogger<EasyAuthProvider> logger) : IEasyAuthProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ILogger<EasyAuthProvider> _logger = logger;
    private HttpContext? HttpContext => _httpContextAccessor.HttpContext;

    // Identity-related headers
    public string? ClientPrincipal =>
        HttpContext?.Request.Headers["X-MS-CLIENT-PRINCIPAL"].FirstOrDefault();
    public string? ClientPrincipalId =>
        HttpContext?.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"].FirstOrDefault();
    public string? ClientPrincipalName =>
    HttpContext?.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].FirstOrDefault();
    public string? ClientPrincipalIdp =>
        HttpContext?.Request.Headers["X-MS-CLIENT-PRINCIPAL-IDP"].FirstOrDefault();

    // Token-related headers
    public string? IdToken =>
        HttpContext?.Request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"].FirstOrDefault();
    public string? AccessToken =>
        HttpContext?.Request.Headers["X-MS-TOKEN-AAD-ACCESS-TOKEN"].FirstOrDefault();
    public string? ExpiresOn =>
        HttpContext?.Request.Headers["X-MS-TOKEN-AAD-EXPIRES-ON"].FirstOrDefault();
    public string? RefreshToken =>
        HttpContext?.Request.Headers["X-MS-TOKEN-AAD-REFRESH-TOKEN"].FirstOrDefault();


    // Helper properties
    public bool IsAuthenticated =>
        !string.IsNullOrEmpty(ClientPrincipal);

    public string? RawClientPrincipalData
    {
        get 
        {
            if (string.IsNullOrEmpty(ClientPrincipal))
            {
                return null;
            }

            try
            {
                return System.Text.Encoding.UTF8.GetString(
                    Convert.FromBase64String(ClientPrincipal));
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse client principal data");
                return null;
            }

        }
    }

    public EasyAuthClientPrincipal? ClientPrincipalData
    {
        get
        {
            if (string.IsNullOrEmpty(RawClientPrincipalData))
            {
                return null;
            }

            try
            {
                return JsonSerializer.Deserialize<EasyAuthClientPrincipal>(RawClientPrincipalData);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse client principal data");
                return null;
            }
        }
    }

}
