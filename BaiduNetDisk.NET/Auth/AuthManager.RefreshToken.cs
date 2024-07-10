using System.Net.Http.Json;

namespace BaiduNetDisk.NET.Auth;

internal partial class AuthManager
{
    private const string RefreshTokenUrl = Constants.ApiHost + "/oauth/2.0/token";
    private async Task RefreshTokenAsync(AccountTokenInfo accountTokenInfo)
    {
        switch (configuration.AuthMode)
        {
            case AuthMode.DeviceCode:
                await RefreshTokenInternalAsync(accountTokenInfo).ConfigureAwait(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async Task RefreshTokenInternalAsync(AccountTokenInfo accountTokenInfo)
    {
        var client = new HttpClient();
        var response = await client.GetAsync(
            $"{RefreshTokenUrl}?grant_type=refresh_token&refresh_token={accountTokenInfo.RefreshToken}&client_id={configuration.AppKey}&client_secret={configuration.SecretKey}"
            ).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<AccessTokenResponse>().ConfigureAwait(false);
        if (content == null)
        {
            throw new Exception("Failed to refresh token");
        }
        
        AccountTokenInfo = new AccountTokenInfo(
            content.AccessToken,
            content.ExpiresIn,
            content.RefreshToken + DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            content.Scope
        );
        await AccountTokenProvider.CacheTokenInfoAsync(AccountTokenInfo).ConfigureAwait(false);
    }
}