using System.Text.Json.Serialization;

namespace BaiduNetDisk.NET.Storage;

public record StorageInfo(
    [property: JsonPropertyName("total")]
    long Total,
    
    [property: JsonPropertyName("expire")]
    bool Expire,
    
    [property: JsonPropertyName("used")]
    long Used,
    
    [property: JsonPropertyName("free")]
    long Free
);
