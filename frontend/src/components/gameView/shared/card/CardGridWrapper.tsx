import type { CardData } from "../../../../interfaces/CardData.ts";
import Card from "./Card.tsx";

interface Props {
  className?: string;
  cards: CardData[];
}

const CardGridWrapper = ({ cards, className }: Props) => {
  return (
    <div className={`grid grid-cols-4 ${className}`}>
      {cards.map((card, i) => (
        <Card key={i} card={card} />
      ))}
    </div>
  );
};

export default CardGridWrapper;
