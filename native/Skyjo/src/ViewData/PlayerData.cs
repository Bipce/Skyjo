using System.Text.Json.Serialization;

namespace Skyjo.ViewData;

[JsonSerializable(typeof(PlayerData))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class AppJsonContext : JsonSerializerContext { }

public  sealed class PlayerData
{
    public string Username { get; set; } = null!;
    public bool IsOwner { get; set; }
}