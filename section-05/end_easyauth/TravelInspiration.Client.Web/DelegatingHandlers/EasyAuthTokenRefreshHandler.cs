using System.Globalization;
using TravelInspiration.Client.Web.Services;

namespace TravelInspiration.Client.Web.DelegatingHandlers;

public class EasyAuthTokenRefreshHandler(
    IEasyAuthProvider easyAuthProvider,
    IHttpContextAccessor httpContextAccessor,
    IHttpClientFactory httpClientFactory,
    ILogger<EasyAuthTokenRefreshHandler> logger) : DelegatingHandler
{
    private readonly IEasyAuthProvider _easyAuthProvider = easyAuthProvider;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<EasyAuthTokenRefreshHandler> _logger = logger;
    private readonly TimeSpan _skewTime = new TimeSpan(0, 120, 0);

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Check if token needs refresh before making the request
        if (ShouldRefreshToken())
        {
            _logger.LogInformation("Token is near expiration, attempting refresh before request");
            await RefreshTokenAsync(cancellationToken);
        }

        // Continue with the next handler 
        return await base.SendAsync(request, cancellationToken);
    }

    private bool ShouldRefreshToken()
    {
        // Use ExpiresOn string instead of TokenExpirationTime
        var expiresOnString = _easyAuthProvider.ExpiresOn;
        if (string.IsNullOrEmpty(expiresOnString))
        {
            _logger.LogWarning("ExpiresOn value is not available");
            return false;
        }

        try
        {
            // Parse the ExpiresOn string which is in UTC format like "2025-09-30T10:53:31.2370400Z"
            if (!DateTime.TryParse(expiresOnString, null, DateTimeStyles.RoundtripKind, out var expirationTime))
            {
                _logger.LogWarning("Failed to parse ExpiresOn value: {ExpiresOn}", expiresOnString);
                return false;
            }

            // Ensure we're working with UTC time
            if (expirationTime.Kind != DateTimeKind.Utc)
            {
                expirationTime = expirationTime.ToUniversalTime();
            }

            var refreshThreshold = expirationTime.Subtract(_skewTime);

            return DateTime.UtcNow >= refreshThreshold;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating ShouldRefreshToken boolean");
            return false;
        }
    }
    private async Task<bool> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                _logger.LogWarning("HttpContext is not available for token refresh");
                return false;
            }

            // Create a new HTTP client for the refresh request
            var refreshClient = _httpClientFactory.CreateClient();

            // Copy the authentication cookies from the current request
            var cookies = httpContext.Request.Headers.Cookie.ToString();
            if (!string.IsNullOrEmpty(cookies))
            {
                refreshClient.DefaultRequestHeaders.Add("Cookie", cookies);
            }

            // Set the base address to the current request's host
            var scheme = httpContext.Request.Scheme;
            var host = httpContext.Request.Host;
            refreshClient.BaseAddress = new Uri($"{scheme}://{host}");

            _logger.LogDebug("Calling /.auth/refresh endpoint");

            var refreshResponse = await refreshClient.GetAsync("/.auth/refresh", cancellationToken);

            if (refreshResponse.IsSuccessStatusCode)
            {
                _logger.LogInformation("Token refresh successful");
                return true;
            }
            else
            {
                _logger.LogWarning("Token refresh failed with status: {StatusCode}", refreshResponse.StatusCode);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return false;
        }
    }

}