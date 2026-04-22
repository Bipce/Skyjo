using System.Text.Json.Serialization.Metadata;

namespace Skyjo.ViewData;

public sealed class CardData : ViewData
{
    protected override JsonTypeInfo JsonTypeInfo => AppJsonContext.Default.CardData;

    public int Number { get; set; }
    public bool IsRevealed { get; set; }
    public bool WillBeRevealed { get; set; }

    public static CardData Empty => new();
}