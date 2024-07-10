using System.Net.Http.Json;
using BaiduNetDisk.NET.User;

namespace BaiduNetDisk.NET;

public partial class BaiduNetDiskManager
{
    private const string UserProfileUrl = "https://pan.baidu.com/rest/2.0/xpan/nas?method=uinfo";

    /*
     * <exception cref="Exception">Failed to get user profile</exception>
     */
    public async Task<UserProfile> GetUserProfileAsync()
    {
        var client = new HttpClient();
        var response = await client
            .GetAsync($"{UserProfileUrl}&access_token={AuthManager.AccountTokenInfo!.AccessToken}")
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<UserProfile>().ConfigureAwait(false);
        if (content == null)
        {
            throw new Exception("Failed to get user profile");
        }

        return content;
    }
}
