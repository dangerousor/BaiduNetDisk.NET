using BaiduNetDisk.NET.Auth;

namespace BaiduNetDisk.NET;

public record Configuration(
    AuthMode AuthMode,
    string AppKey,
    string SecretKey,
    string SignKey,
    string? RedirectUri = null
);
