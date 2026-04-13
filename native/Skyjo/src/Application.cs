using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Skyjo.Network;
using Ultralight.FNA;

#if !DEBUG
using SteamDatabase.ValvePak;
#endif

namespace Skyjo;

public sealed class Application : Game
{
    private UltralightRenderer _renderer = null!;
    private UltralightView _view = null!;

    private SpriteBatch _spriteBatch = null!;

    private int CurrentWidth => GraphicsDevice.Viewport.Width;
    private int CurrentHeight => GraphicsDevice.Viewport.Height;

    private NetworkManager NetworkManager { get; } = new();

    private KeyboardState _keyboard;
    private KeyboardState _lastKeyboard;

    private Texture2D _pixelTexture = null!;

    public Application()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;

        NetworkManager.RegisterEntity<TestEntity>();
        NetworkManager.RegisterEntity<Player>();
        NetworkManager.RegisterEntity<GameManager>();

        NetworkManager.ServerManager.OnPlayerConnected += Server_OnPlayerConnected;
        NetworkManager.ServerManager.OnServerStarted += Server_OnStarted;

        NetworkManager.ClientManager.ConnectionData = writer =>
        {
            var color = new Color(Random.Shared.NextSingle(), Random.Shared.NextSingle(),
                Random.Shared.NextSingle());
            writer.Put(color.R);
            writer.Put(color.G);
            writer.Put(color.B);
        };
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

        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData([Color.White]);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        NetworkManager.Update();

        UpdateInput();

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

        _spriteBatch.Begin();

        var i = 0;
        foreach (var _ in NetworkManager.GetEntities<TestEntity>())
        {
            _spriteBatch.Draw(_pixelTexture, new Rectangle(i * 50, 0, 50, 50), Color.White);
            i++;
        }

        i = 0;
        foreach (var player in NetworkManager.GetEntities<Player>())
        {
            var color = player.IsOwner ? Color.Blue : Color.Red;
            _spriteBatch.Draw(_pixelTexture, new Rectangle(i * 50, 50, 50, 50), color);
            i++;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

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

    private void UpdateInput()
    {
        _lastKeyboard = _keyboard;
        _keyboard = Keyboard.GetState();

        if (IsKeyJustPressed(Keys.H)) // Host
        {
            if (NetworkManager.ServerManager.Start())
                NetworkManager.ClientManager.Start();
        }

        if (IsKeyJustPressed(Keys.S)) // Server
            NetworkManager.ServerManager.Start();
        if (IsKeyJustPressed(Keys.C)) // Client
        {
            NetworkManager.ClientManager.Start();
        }

        if (IsKeyJustPressed(Keys.D)) // Disconnect
            NetworkManager.Stop();

        if (IsKeyJustPressed(Keys.Enter) && NetworkManager.IsRunning)
        {
            var gameManager = NetworkManager.GetEntities<GameManager>().First();
            gameManager.Server_SpawnEntity();
        }
    }

    private bool IsKeyJustPressed(Keys key) => _keyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    private void Server_OnPlayerConnected(NetPeer peer, NetDataReader reader)
    {
        var color = new Color(reader.GetByte(), reader.GetByte(), reader.GetByte());

        var player = new Player
        {
            Owner = peer,
            Color = color
        };
        player.Spawn();
    }

    private void Server_OnStarted()
    {
        new GameManager().Spawn();
    }
}