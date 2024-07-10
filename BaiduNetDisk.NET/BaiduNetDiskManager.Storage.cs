using System.Net.Http.Json;
using BaiduNetDisk.NET.Storage;

namespace BaiduNetDisk.NET;

public partial class BaiduNetDiskManager
{
    private const string StorageInfoUrl = "https://pan.baidu.com/api/quota";
    
    public async Task<StorageInfo> GetStorageInfoAsync(bool checkFree = false, bool checkExpire = false)
    {
        var free = checkFree ? "1" : "0";
        var expire = checkExpire ? "1" : "0";
        var client = new HttpClient();
        var response = await client.GetAsync(
            $"{StorageInfoUrl}?access_token={AuthManager.AccountTokenInfo!.AccessToken}&checkfree={free}&checkexpire={expire}"
            ).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<StorageInfo>().ConfigureAwait(false);
        if (content == null)
        {
            throw new Exception("Failed to get storage information");
        }
        
        return content;
    }
}
