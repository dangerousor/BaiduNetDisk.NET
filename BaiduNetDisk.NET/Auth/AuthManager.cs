using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace BaiduNetDisk.NET.Auth;

internal partial class AuthManager(
    IAccountTokenProvider accountTokenProvider,
    ILoggerFactory loggerFactory,
    Configuration configuration
)
{
    private IAccountTokenProvider AccountTokenProvider { get; } = accountTokenProvider;

    private ILogger<AuthManager> Logger { get; } = loggerFactory.CreateLogger<AuthManager>();

    public AccountTokenInfo? AccountTokenInfo { get; set; }
    
    /*
     * Throws:
     * - Exception: Failed to get device code
     */
    public async Task EnsureAuthenticatedAsync()
    {
        AccountTokenInfo? tokenInfo = null;
        try
        {
            tokenInfo = await AccountTokenProvider.ExtractTokenInfoAsync().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Failed to extract account token information");
        }

        if (tokenInfo == null)
        {
            Logger.LogDebug("Empty account token information");
            await AuthenticateAsync().ConfigureAwait(false);
            return;
        }

        if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= tokenInfo.ExpiresIn)
        {
            Logger.LogDebug("Account token is expired");
            await RefreshTokenAsync(tokenInfo).ConfigureAwait(false);
            return;
        }

        AccountTokenInfo = tokenInfo;
        Logger.LogDebug("Account token is valid");
    }

    private async Task AuthenticateAsync()
    {
        switch (configuration.AuthMode)
        {
            case AuthMode.DeviceCode:
                await AuthenticateDeviceCodeAsync().ConfigureAwait(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private record AccessTokenResponse(
        [property: JsonPropertyName("access_token")]
        string AccessToken,
        
        [property: JsonPropertyName("expires_in")]
        int ExpiresIn,
        
        [property: JsonPropertyName("refresh_token")]
        string RefreshToken,
        
        [property: JsonPropertyName("scope")]
        string Scope
    );
}
