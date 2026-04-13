using Microsoft.Xna.Framework;
using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed partial class Player : Entity
{
    public Color Color { get; init; } // todo: Replicated

    [Client]
    public void Client_SayHello()
    {
        Console.WriteLine($"Hello: {Random.Shared.Next()}");
    }
}