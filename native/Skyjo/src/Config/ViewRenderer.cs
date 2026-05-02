using System.Text.Json.Serialization;

namespace Skyjo.Config;

[JsonConverter(typeof(JsonStringEnumConverter<ViewRenderer>))]
public enum ViewRenderer
{
    SdlGpu,
    D3D11,
    Cpu
}