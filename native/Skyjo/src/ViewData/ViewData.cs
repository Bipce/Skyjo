using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Skyjo.ViewData;

public abstract class ViewData
{
    protected abstract JsonTypeInfo JsonTypeInfo { get; }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this, JsonTypeInfo);
    }
}