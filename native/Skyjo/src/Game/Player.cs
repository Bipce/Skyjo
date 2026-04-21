using Microsoft.Xna.Framework;
using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo.Game;

public sealed partial class Player : Entity
{
    [Replicated] public Color Color { get; set; }
}