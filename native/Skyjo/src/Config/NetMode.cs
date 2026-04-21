using System.Text.Json.Serialization;

namespace Skyjo.Config;

[JsonConverter(typeof(JsonStringEnumConverter<NetMode>))]
public enum NetMode
{
    Host,
    Join
}