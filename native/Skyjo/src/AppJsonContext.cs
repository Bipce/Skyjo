using System.Text.Json.Serialization;
using Skyjo.ViewData;

namespace Skyjo;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(PlayerData))]
[JsonSerializable(typeof(CardData))]
internal partial class AppJsonContext : JsonSerializerContext;