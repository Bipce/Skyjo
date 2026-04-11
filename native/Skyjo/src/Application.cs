using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteamDatabase.ValvePak;
using Ultralight.FNA;

namespace Skyjo;

public sealed class Application : Game
{
    private UltralightRenderer _renderer = null!;
    private UltralightView _view = null!;

    private SpriteBatch _spriteBatch = null!;

    private int CurrentWidth => GraphicsDevice.Viewport.Width;
    private int CurrentHeight => GraphicsDevice.Viewport.Height;

    public Application()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
    }

    private void OnResize(object? sender, EventArgs e)
    {
        _view.Resize(CurrentWidth, CurrentHeight);
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _spriteBatch = new SpriteBatch(GraphicsDevice);

#if DEBUG
        const string url = "http://localhost:5173";
        _renderer = new UltralightRendererSDLGPU(GraphicsDevice, assetsDir: "data", enableLog: true);
#else
        const string url = "file:///index.html";
        _renderer = new UltralightRendererSDLGPU(GraphicsDevice, fileSystem: new VpkFileSystem("data/ui.vpk"),
            shaders: GetShaders(), enableLog: false);
#endif
        _view = new UltralightView(_renderer, CurrentWidth, CurrentHeight);
        _view.LoadUrl(url);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!IsActive)
        {
            return;
        }

        _renderer.Update();
        _view.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _renderer.Render();
        _view.Render();

        _spriteBatch.Begin();
        _spriteBatch.Draw(_view.Texture, Vector2.Zero, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

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
}