using System.Text.Json;

namespace Skyjo.Config;

public sealed class ConfigManager
{
    public Settings Settings { get; private set; } = null!;
    
    public void Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "settings.json");
        var json = File.ReadAllText(path);
        Settings = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.Settings)!;
    }
}