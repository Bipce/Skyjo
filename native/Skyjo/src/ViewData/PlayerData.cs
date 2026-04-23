using System.Text.Json.Serialization.Metadata;

namespace Skyjo.ViewData;

public sealed class PlayerData : ViewData
{
    protected override JsonTypeInfo JsonTypeInfo => AppJsonContext.Default.PlayerData;

    public string Username { get; set; } = null!;
    public bool IsOwner { get; set; }
    public CardData[] Cards { get; init; } = null!;
}