using BaiduNetDisk.NET.Auth;
using BaiduNetDisk.NET.Storage;
using BaiduNetDisk.NET.User;
using Microsoft.Extensions.Logging;

namespace BaiduNetDisk.NET;

public partial class BaiduNetDiskManager(
    IAccountTokenProvider accountTokenProvider,
    ILoggerFactory loggerFactory,
    Configuration configuration)
{
    private AuthManager AuthManager { get; } = new(accountTokenProvider, loggerFactory, configuration);

    private ILogger<BaiduNetDiskManager> Logger { get; } = loggerFactory.CreateLogger<BaiduNetDiskManager>();
    
    public event EventHandler<DeviceCodeEventArgs>? DeviceCodeReceived;
    
    private void OnDeviceCodeReceived(object? _, DeviceCodeEventArgs e)
    {
        DeviceCodeReceived?.Invoke(this, e);
    }
    
    /*
     * <exception cref="Exception">Failed to get device code</exception>
     */
    public async Task EnsureAuthenticatedAsync()
    {
        RegisterEvents();
        
        await AuthManager.EnsureAuthenticatedAsync();
        
        UnregisterEvents();
    }
    
    private void RegisterEvents()
    {
        AuthManager.DeviceCodeReceived -= OnDeviceCodeReceived;
        AuthManager.DeviceCodeReceived += OnDeviceCodeReceived;
    }
    
    private void UnregisterEvents()
    {
        AuthManager.DeviceCodeReceived -= OnDeviceCodeReceived;
    }
}
