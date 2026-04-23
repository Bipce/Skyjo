import type { CardBelongToType, CardData } from "../../../../interfaces/CardData.ts";
import Card from "./Card.tsx";

interface Props {
  className?: string;
  cards: CardData[];
  belongsTo: CardBelongToType;
  onSelectedForRevealCard?: (id: number) => void;
}

const CardGridWrapper = ({ cards, belongsTo, onSelectedForRevealCard, className }: Props) => {
  return (
    <div className={`grid grid-cols-4 ${className}`}>
      {cards.map(card => (
        <Card
          key={card.id}
          card={card}
          belongsTo={belongsTo}
          handleOnClick={onSelectedForRevealCard ? () => onSelectedForRevealCard(card.id) : undefined}
        />
      ))}
    </div>
  );
};

export default CardGridWrapper;
