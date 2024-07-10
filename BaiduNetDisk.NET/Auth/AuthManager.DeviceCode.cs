using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace BaiduNetDisk.NET.Auth;

internal partial class AuthManager
{
    private const string DeviceCodeUrl = $"{Constants.ApiHost}/oauth/2.0/device/code";
    private const string DeviceCodeExchangeUrl = $"{Constants.ApiHost}/oauth/2.0/token";
    
    public event EventHandler<DeviceCodeEventArgs>? DeviceCodeReceived;

    private async Task AuthenticateDeviceCodeAsync()
    {
        var client = new HttpClient();
        var response = await client.GetAsync(
            $"{DeviceCodeUrl}?response_type=device_code&client_id={configuration.AppKey}&scope=basic,netdisk"
            ).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<DeviceCodeResponse>().ConfigureAwait(false);
        if (content == null)
        {
            throw new Exception("Failed to get device code");
        }
        Logger.LogInformation("Device Code Response: {Response}", content);
        DeviceCodeReceived?.Invoke(
            null,
            new DeviceCodeEventArgs(content.UserCode, content.VerificationUrl, content.QrCodeUrl)
            );

        await DeviceCodeExchangeTokenAsync(content).ConfigureAwait(false);
    }

    private async Task DeviceCodeExchangeTokenAsync(DeviceCodeResponse response)
    {
        var startTime = DateTimeOffset.UtcNow;
        while (DateTimeOffset.UtcNow - startTime < TimeSpan.FromSeconds(response.ExpiresIn))
        {
            try
            {
                await DeviceCodeExchangeTokenRequestAsync(response).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Failed to exchange token");
                await Task.Delay(1000 * response.Interval).ConfigureAwait(false);
                continue;
            }
            
            break;
        }
    }

    private async Task DeviceCodeExchangeTokenRequestAsync(DeviceCodeResponse response)
    {
        var client = new HttpClient();
        var tokenResponse = await client
            .GetAsync(
                $"{DeviceCodeExchangeUrl}?grant_type=device_token&code={response.DeviceCode}&client_id={configuration.AppKey}&client_secret={configuration.SecretKey}")
            .ConfigureAwait(false);
        tokenResponse.EnsureSuccessStatusCode();
        var tokenContent = await tokenResponse.Content.ReadFromJsonAsync<AccessTokenResponse>().ConfigureAwait(false);
        if (tokenContent == null)
        {
            throw new Exception("Failed to exchange token");
        }

        AccountTokenInfo = new AccountTokenInfo(tokenContent.AccessToken, tokenContent.ExpiresIn,
            tokenContent.RefreshToken + DateTimeOffset.UtcNow.ToUnixTimeSeconds(), tokenContent.Scope);
        await AccountTokenProvider.CacheTokenInfoAsync(AccountTokenInfo).ConfigureAwait(false);
    }

    private record DeviceCodeResponse(
        [property: JsonPropertyName("device_code")]
        string DeviceCode,
        
        [property: JsonPropertyName("user_code")]
        string UserCode,
        
        [property: JsonPropertyName("verification_url")]
        string VerificationUrl,
        
        [property: JsonPropertyName("qrcode_url")]
        string QrCodeUrl,
        
        [property: JsonPropertyName("expires_in")]
        int ExpiresIn,
        
        [property: JsonPropertyName("interval")]
        int Interval
    );
}