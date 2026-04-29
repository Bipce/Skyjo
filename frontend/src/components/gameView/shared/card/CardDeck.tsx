import Card from "./Card.tsx";
import { useGameStore } from "../../../../store/gameStore.ts";
import type { CardData } from "../../../../interfaces/CardData.ts";

interface Props {
  drawnCard: CardData;
  discardedCard: CardData;
}

const CardDeck = ({ drawnCard, discardedCard }: Props) => {
  const hasDiscardedDrawnCard = useGameStore(s => s.hasDiscardedDrawnCard);

  return (
    <section className="flex items-center justify-center gap-10">
      <Card
        isDraggable={discardedCard.isRevealed && !drawnCard.isRevealed && !hasDiscardedDrawnCard}
        isDroppable={drawnCard.isRevealed}
        card={discardedCard}
        belongsTo="deck"
        isDiscarded
      />
      <Card isDraggable={drawnCard.isRevealed} card={drawnCard} belongsTo="deck" />
    </section>
  );
};

export default CardDeck;
