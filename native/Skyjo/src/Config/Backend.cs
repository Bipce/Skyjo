using System.Text.Json.Serialization;

namespace Skyjo.Config;

[JsonConverter(typeof(JsonStringEnumConverter<Backend>))]
public enum Backend
{
    SdlGpu,
    D3D11
}