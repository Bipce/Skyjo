import type { CardBelongToType, CardData } from "../../../../interfaces/CardData.ts";
import Card from "./Card.tsx";

interface Props {
  className?: string;
  cards: CardData[];
  belongsTo: CardBelongToType;
}

const CardGridWrapper = ({ cards, belongsTo, className }: Props) => {
  return (
    <div className={`grid grid-cols-4 ${className}`}>
      {cards.map(card => (
        <Card key={card.id} card={card} belongsTo={belongsTo} />
      ))}
    </div>
  );
};

export default CardGridWrapper;
