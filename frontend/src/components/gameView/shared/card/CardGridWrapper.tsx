import type { CardBelongToType, CardData } from "../../../../interfaces/CardData.ts";
import Card from "./Card.tsx";

interface Props {
  className?: string;
  cards: CardData[];
  belongsTo: CardBelongToType;
  onSelectedForRevealCard: (index: number) => void;
  selectedCardsForInitiateGame: number[];
}

const CardGridWrapper = ({
  cards,
  belongsTo,
  selectedCardsForInitiateGame,
  onSelectedForRevealCard,
  className,
}: Props) => {
  return (
    <div className={`grid grid-cols-4 ${className}`}>
      {cards.map((card, i) => (
        <Card
          key={i}
          card={card}
          belongsTo={belongsTo}
          handleOnClick={() => onSelectedForRevealCard(i)}
          isSelected={selectedCardsForInitiateGame.includes(i)}
        />
      ))}
    </div>
  );
};

export default CardGridWrapper;
