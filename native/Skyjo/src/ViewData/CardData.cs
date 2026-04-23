using System.Text.Json.Serialization.Metadata;

namespace Skyjo.ViewData;

public sealed class CardData : ViewData
{
    protected override JsonTypeInfo JsonTypeInfo => AppJsonContext.Default.CardData;

    public ushort Id { get; set; }
    public int Number { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsSelected { get; set; }

    public static CardData Empty => new();
}