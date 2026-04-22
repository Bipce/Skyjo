import Card from "./Card.tsx";
import type { CardData } from "../../../../interfaces/CardData.ts";

interface Props {
  drawnCard: CardData;
  discardedCard: CardData;
}

const CardDeck = ({ drawnCard, discardedCard }: Props) => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Card card={drawnCard} belongsTo="deck" />
      <Card card={discardedCard} belongsTo="deck" />
    </section>
  );
};

export default CardDeck;
