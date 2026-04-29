using System.Text.Json.Serialization.Metadata;
using Skyjo.Enums;

namespace Skyjo.ViewData;

public sealed class CardData : ViewData
{
    protected override JsonTypeInfo JsonTypeInfo => AppJsonContext.Default.CardData;

    public ushort Id { get; set; }
    public sbyte Number { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsSelected { get; set; }
    public bool IsHighlighted { get; set; }
    public bool HasDoublePoint { get; set; }
    public CardType Type { get; set; }

    public static CardData Empty => new();
}