using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Skyjo.Config;
using Ultralight.FNA;
#if !DEBUG
using SteamDatabase.ValvePak;
#endif

namespace Skyjo.Game;

public sealed partial class GameView
{
    private static GameView _instance = null!;

    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly Settings _settings;
    private readonly UltralightRenderer _renderer;
    private readonly UltralightView _view;

    public GameView(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Settings settings)
    {
        _instance = this;

        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;
        _settings = settings;

#if DEBUG
        const string url = "http://localhost:5173";
        _renderer = new UltralightRendererSDLGPU(_graphicsDevice, assetsDir: "data", enableLog: true,
            msaaCount: (uint)settings.MsaaCount);
#else
        const string url = "file:///index.html";
        _renderer = _settings.ViewRenderer switch
        {
            ViewRenderer.SdlGpu => new UltralightRendererSDLGPU(_graphicsDevice,
                fileSystem: new VpkFileSystem("data/ui.vpk"), shaders: GetShaders(), enableLog: false,
                msaaCount: (uint)settings.MsaaCount),
            ViewRenderer.D3D11 => new UltralightRendererD3D11(_graphicsDevice,
                fileSystem: new VpkFileSystem("data/ui.vpk"), shaders: GetShaders(), enableLog: false,
                msaaCount: (uint)settings.MsaaCount),
            ViewRenderer.Cpu => new UltralightRendererCPU(_graphicsDevice, fileSystem: new VpkFileSystem("data/ui.vpk"),
                enableLog: false),
            _ => _renderer
        } ?? throw new InvalidOperationException("Invalid view renderer");

#endif
        _view = new UltralightView(_renderer, CurrentWidth, CurrentHeight);
        _view.LoadUrl(url);
    }

    private int CurrentWidth => _graphicsDevice.Viewport.Width;
    private int CurrentHeight => _graphicsDevice.Viewport.Height;

    public static UltralightView View => _instance._view;

#if !DEBUG
    private ShaderSources GetShaders()
    {
        var package = new Package();
        package.OptimizeEntriesForBinarySearch(StringComparison.OrdinalIgnoreCase);
        package.Read("data/shaders.vpk");

        if (_settings.ViewRenderer == ViewRenderer.SdlGpu)
        {
            return new ShaderSources
            {
                FillVert = GetDataInPackage(package, "sdlgpu/fill.vert.spv"),
                FillFrag = GetDataInPackage(package, "sdlgpu/fill.frag.spv"),
                PathVert = GetDataInPackage(package, "sdlgpu/fill_path.vert.spv"),
                PathFrag = GetDataInPackage(package, "sdlgpu/fill_path.frag.spv"),
            };
        }

        if (_settings.ViewRenderer == ViewRenderer.D3D11)
        {
            return new ShaderSources
            {
                FillVert = GetDataInPackage(package, "d3d11/v2f_c4f_t2f_t2f_d28f.hlsl"),
                FillFrag = GetDataInPackage(package, "d3d11/fill.hlsl"),
                PathVert = GetDataInPackage(package, "d3d11/v2f_c4f_t2f.hlsl"),
                PathFrag = GetDataInPackage(package, "d3d11/fill_path.hlsl"),
            };
        }

        throw new InvalidOperationException("Invalid view renderer");
    }

    private static byte[] GetDataInPackage(Package package, string path)
    {
        var entry = package.FindEntry(path);
        if (entry == null)
            throw new FileNotFoundException(path);

        package.ReadEntry(entry, out var data);
        return data;
    }
#endif

    public void OnResize()
    {
        _view.Resize(CurrentWidth, CurrentHeight);
    }

    public void Update()
    {
        _renderer.Update();
        _view.Update();
    }

    public void Render()
    {
        _renderer.Render();
        _view.Render();
    }

    public void Draw()
    {
        _spriteBatch.Draw(_view.Texture, Vector2.Zero, Color.White);
    }
}