import Card from "./Card.tsx";
import type { CardData } from "../../../../interfaces/CardData.ts";

interface Props {
  drawnCard: CardData;
  discardedCard: CardData;
}

const CardDeck = ({ drawnCard, discardedCard }: Props) => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Card
        isDraggable={discardedCard.isRevealed && !drawnCard.isRevealed}
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
