using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Skyjo.Config;
using Skyjo.Game;
using Skyjo.Network;
using ConnectionState = Skyjo.Network.Enums.ConnectionState;

#if !DEBUG
using SDL3;
#endif

namespace Skyjo;

public sealed class Application : Microsoft.Xna.Framework.Game
{
#if DEBUG
    static Application()
    {
        for (var i = 1; i <= 3; i++)
        {
            var mutex = new Mutex(true, $"Skyjo_Instance_{i}", out var created);
            if (created)
            {
                InstanceNumber = i;
                break;
            }

            mutex.Dispose();
        }
    }

    private static readonly int InstanceNumber;
#endif

    private GameView _gameView = null!;
    private TestView _testView = null!;

    private SpriteBatch _spriteBatch = null!;

    private NetworkManager NetworkManager { get; } = new();
    private ConfigManager ConfigManager { get; } = new();

    private GameManager? _gameManager;

    public Application()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = 1280;
        graphics.PreferredBackBufferHeight = 720;
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnResize;
        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = true;

        NetworkManager.RegisterEntity<GameManager>();
        NetworkManager.RegisterEntity<Player>();
        NetworkManager.RegisterEntity<Card>();

        NetworkManager.ServerManager.OnPlayerConnected += Server_OnPlayerConnected;
        NetworkManager.ServerManager.ConnectionStateChangedEvent += OnServerConnectionStateChanged;

        NetworkManager.ClientManager.ConnectionData = writer => writer.Put(ConfigManager.Settings.Username);

        ConfigManager.Load();
    }

    private void OnResize(object? sender, EventArgs e)
    {
        _gameView.OnResize();
    }

    protected override void Initialize()
    {
        base.Initialize();

#if !DEBUG
        SDL.SDL_MaximizeWindow(Window.Handle);
#endif
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _gameView = new GameView(GraphicsDevice, _spriteBatch);
        _testView = new TestView(GraphicsDevice, _spriteBatch);

        GameView.BindFunction("startNetwork", StartNetwork);
    }

    private void StartNetwork()
    {
        if (NetworkManager.IsRunning)
            return;

#if DEBUG
        if (InstanceNumber == 1)
            NetworkManager.ServerManager.Start();
        ConfigManager.Settings.Username = $"Player {InstanceNumber}";
        NetworkManager.ClientManager.Start();
#else
        var settings = ConfigManager.Settings;
        switch (ConfigManager.Settings.NetMode)
        {
            case NetMode.Host:
            {
                NetworkManager.ServerManager.Port = settings.Port;
                NetworkManager.Host();
                break;
            }
            case NetMode.Join:
                NetworkManager.ClientManager.Address = settings.Address;
                NetworkManager.ClientManager.Port = settings.Port;
                NetworkManager.ClientManager.Start();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
#endif
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        NetworkManager.Update(gameTime);

        _gameView.Update();
        _testView.Update();

        _gameManager?.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _gameView.Render();
        _testView.Draw();

        _spriteBatch.Begin();
        _gameView.Draw();
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void Server_OnPlayerConnected(NetPeer peer, NetDataReader reader)
    {
        var username = reader.GetString();

        var player = new Player
        {
            Owner = peer,
            Username = username
        };
        player.Spawn();
    }

    private void OnServerConnectionStateChanged(ConnectionState connectionState)
    {
        if (connectionState != ConnectionState.Started)
            return;

        _gameManager = new GameManager();
        _gameManager.Spawn();
    }
}