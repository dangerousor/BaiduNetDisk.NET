namespace BaiduNetDisk.NET.Auth;

public record DeviceCodeEventArgs(
    string UserCode,
    string VerificationUrl,
    string QrCodeUrl
);
