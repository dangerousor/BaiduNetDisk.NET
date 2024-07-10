namespace BaiduNetDisk.NET.Auth;

/*
 * To be implemented by the client to provide the account token information.
 * The client should implement this interface to provide the account token information.
 * The client should implement the ExtractTokenInfo method to extract the account token information from cache.
 * The client should implement the CacheTokenInfo method to cache the account token information.
 */
public interface IAccountTokenProvider
{
    /*
     * Extract the account token information from cache.
     * This method will be called by the SDK to extract the account token information from cache.
     * E.g. At the start of the SDK, the SDK will call this method to extract the account token information from cache.
     * 
     * @return The account token information.
     */
    public Task<AccountTokenInfo?> ExtractTokenInfoAsync();
    
    /*
     * Cache the account token information.
     * This method will be called by the SDK to cache the account token information.
     * E.g. After the SDK gets the account token information from the server, the SDK will call this method to cache the account token information.
     * 
     * @param tokenInfo The account token information to be cached.
     */
    public Task CacheTokenInfoAsync(AccountTokenInfo tokenInfo);
}
