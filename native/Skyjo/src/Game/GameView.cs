using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ultralight.FNA;
#if !DEBUG
using SteamDatabase.ValvePak;
#endif

namespace Skyjo.Game;

public sealed class GameView
{
    private static GameView _instance = null!;

    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly UltralightRenderer _renderer;
    private readonly UltralightView _view;

    public GameView(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _instance = this;

        _graphicsDevice = graphicsDevice;
        _spriteBatch = spriteBatch;

#if DEBUG
        const string url = "http://localhost:5173";
        _renderer = new UltralightRendererSDLGPU(_graphicsDevice, assetsDir: "data", enableLog: true);
#else
        const string url = "file:///index.html";
        _renderer = new UltralightRendererSDLGPU(_graphicsDevice, fileSystem: new VpkFileSystem("data/ui.vpk"),
            shaders: GetShaders(), enableLog: false);
#endif
        _view = new UltralightView(_renderer, CurrentWidth, CurrentHeight);
        _view.LoadUrl(url);
    }

    private int CurrentWidth => _graphicsDevice.Viewport.Width;
    private int CurrentHeight => _graphicsDevice.Viewport.Height;

#if !DEBUG
    private static ShaderSources GetShaders()
    {
        var package = new Package();
        package.OptimizeEntriesForBinarySearch(StringComparison.OrdinalIgnoreCase);
        package.Read("data/shaders.vpk");

        return new ShaderSources
        {
            FillVert = GetDataInPackage(package, "fill.vert.spv"),
            FillFrag = GetDataInPackage(package, "fill.frag.spv"),
            PathVert = GetDataInPackage(package, "fill_path.vert.spv"),
            PathFrag = GetDataInPackage(package, "fill_path.frag.spv"),
        };
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

    public static void EvaluateScript(string js)
    {
        _instance._view.EvaluateScript(js);
    }

    public static void BindFunction(string name, Action callback)
    {
        _instance._view.BindFunction(name, callback);
    }
}