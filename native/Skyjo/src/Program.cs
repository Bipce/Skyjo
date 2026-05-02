#if !DEBUG
using SDL3;
#endif

using Skyjo;
using Skyjo.Config;

ConfigManager configManager = new();
configManager.Load();

#if !DEBUG
if (configManager.Settings.Backend == Backend.D3D11)
{
    SDL.SDL_SetHintWithPriority(
        "FNA3D_FORCE_DRIVER",
        "D3D11",
        SDL.SDL_HintPriority.SDL_HINT_OVERRIDE
    );
}
#endif

using var app = new Application(configManager);
app.Run();