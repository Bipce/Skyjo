using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Skyjo.Network;

namespace Skyjo.Game;

public sealed class TestView
{
    private NetworkManager NetworkManager => NetworkManager.Instance;
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture2D _pixelTexture;

    private KeyboardState _keyboard;
    private KeyboardState _lastKeyboard;

    public TestView(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;

        _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
        _pixelTexture.SetData([Color.White]);
    }

    private bool IsKeyJustPressed(Keys key) => _keyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    public void Update()
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
    }

    public void Draw()
    {
        _spriteBatch.Begin();

        var i = 0;
        foreach (var _ in NetworkManager.GetEntities<TestEntity>())
        {
            _spriteBatch.Draw(_pixelTexture, new Rectangle(i * 50, 0, 50, 50), Color.Gray);
            i++;
        }

        _spriteBatch.End();
    }
}