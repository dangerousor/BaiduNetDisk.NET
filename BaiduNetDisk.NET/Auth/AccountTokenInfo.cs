namespace BaiduNetDisk.NET.Auth;

public record AccountTokenInfo(
    string AccessToken,
    long ExpiresIn,
    string RefreshToken,
    string Scope
);
