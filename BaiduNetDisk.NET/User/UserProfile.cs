using System.Text.Json.Serialization;

namespace BaiduNetDisk.NET.User;

public record UserProfile(
    [property: JsonPropertyName("baidu_name")]
    string BaiduName,
    [property: JsonPropertyName("netdisk_name")]
    string NetdiskName,
    [property: JsonPropertyName("avatar_url")]
    string AvatarUrl,
    [property: JsonPropertyName("vip_type")]
    int VipType,
    [property: JsonPropertyName("uk")] 
    long Uk
);
