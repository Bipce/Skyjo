using System.Text.Json.Serialization;

namespace Skyjo.Config;

[JsonSerializable(typeof(Settings))]
internal partial class SettingsJsonContext : JsonSerializerContext;