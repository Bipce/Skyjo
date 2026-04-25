import type { CardBelongToType, CardData } from "../../../../interfaces/CardData.ts";
import Card from "./Card.tsx";

interface Props {
  className?: string;
  cards: CardData[];
  belongsTo: CardBelongToType;
}

const CardGridWrapper = ({ cards, belongsTo, className }: Props) => {
  const nbrOfColumns = cards.length / 3;

  return (
    <div className={`grid grid-cols-${nbrOfColumns} ${className}`}>
      {cards.map(card => (
        <Card key={card.id} card={card} belongsTo={belongsTo} isDroppable={belongsTo === "player"} />
      ))}
    </div>
  );
};

export default CardGridWrapper;
