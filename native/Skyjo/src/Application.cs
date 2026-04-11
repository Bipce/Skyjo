using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        _renderer = new UltralightRendererSDLGPU(GraphicsDevice);
        _view = new UltralightView(_renderer, CurrentWidth, CurrentHeight);

#if DEBUG
        const string url = "http://localhost:5173";
#else
        const string url = "file:///index.html";
#endif
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
}