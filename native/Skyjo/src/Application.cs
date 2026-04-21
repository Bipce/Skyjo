using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Skyjo.Network;
using Skyjo.Network.Extensions;

#if !DEBUG
using SDL3;
#endif

namespace Skyjo;

public sealed class Application : Game
{
    private GameView _gameView = null!;

    private SpriteBatch _spriteBatch = null!;

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
        graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = true;

        NetworkManager.RegisterEntity<TestEntity>();
        NetworkManager.RegisterEntity<Player>();
        NetworkManager.RegisterEntity<GameManager>();

        NetworkManager.ServerManager.OnPlayerConnected += Server_OnPlayerConnected;
        NetworkManager.ServerManager.OnServerStarted += Server_OnStarted;

        NetworkManager.ClientManager.ConnectionData = writer =>
        {
            var color = new Color(Random.Shared.NextSingle(), Random.Shared.NextSingle(),
                Random.Shared.NextSingle());
            writer.Put(color);
        };
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

        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData([Color.White]);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        NetworkManager.Update(gameTime);

        UpdateInput();

        _gameView.Update();
        foreach (var gameManager in NetworkManager.GetEntities<GameManager>())
        {
            gameManager.Update();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _gameView.Render();

        _spriteBatch.Begin();

        var i = 0;
        foreach (var _ in NetworkManager.GetEntities<TestEntity>())
        {
            _spriteBatch.Draw(_pixelTexture, new Rectangle(i * 50, 0, 50, 50), Color.Gray);
            i++;
        }

        i = 0;
        foreach (var player in NetworkManager.GetEntities<Player>())
        {
            _spriteBatch.Draw(_pixelTexture, new Rectangle(i * 50, 50, 50, 50), player.Color);
            if (player.IsOwner)
                _spriteBatch.Draw(_pixelTexture, new Rectangle(i * 50 + (50 - 10) / 2, 50 + (50 - 10) / 2, 10, 10),
                    Color.DarkRed);
            i++;
        }

        _spriteBatch.End();

        _spriteBatch.Begin();
        _gameView.Draw();
        _spriteBatch.End();

        base.Draw(gameTime);
    }

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
            var gameManager = NetworkManager.GetEntity<GameManager>();
            gameManager.Server_SpawnEntity();
        }

        if (IsKeyJustPressed(Keys.Back) && NetworkManager.IsRunning)
        {
            var gameManager = NetworkManager.GetEntity<GameManager>();
            gameManager.Server_DestroyEntity();
        }

        if (IsKeyJustPressed(Keys.P) && NetworkManager.IsRunning)
        {
            var gameManager = NetworkManager.GetEntity<GameManager>();
            gameManager.Server_IncrementHealth();
        }

        if (IsKeyJustPressed(Keys.O) && NetworkManager.IsRunning)
        {
            var gameManager = NetworkManager.GetEntity<GameManager>();
            gameManager.Server_DecrementHealth();
        }
    }

    private bool IsKeyJustPressed(Keys key) => _keyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    private void Server_OnPlayerConnected(NetPeer peer, NetDataReader reader)
    {
        var color = reader.GetColor();

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