import type { ICard } from "../../interfaces/ICard.ts";
import Card from "./Card";

interface Props {
  className?: string;
  cards: ICard[];
  cardClassName?: string;
}

const CardGridWrapper = ({ cards, className, cardClassName }: Props) => {
  return (
    <div className={`grid grid-cols-4 ${className}`}>
      {cards.map((card, i) => (
        <Card key={i} card={card} cardClassName={cardClassName} />
      ))}
    </div>
  );
};

export default CardGridWrapper;
