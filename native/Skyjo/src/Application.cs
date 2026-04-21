using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Skyjo.Network;
using Skyjo.Network.Extensions;

#if !DEBUG
using SDL3;
#endif

namespace Skyjo;

public sealed class Application : Game
{
    private GameView _gameView = null!;
    private TestView _testView = null!;

    private SpriteBatch _spriteBatch = null!;

    private NetworkManager NetworkManager { get; } = new();

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
        _testView = new TestView(GraphicsDevice, _spriteBatch);
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        NetworkManager.Update(gameTime);

        _gameView.Update();
        _testView.Update();
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