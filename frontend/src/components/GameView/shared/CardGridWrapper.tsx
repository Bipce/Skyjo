import Card from "./Card.tsx";

interface Props {
  className?: string;
  cards: Array<number | string>;
  cardClassName?: string;
}

const CardGridWrapper = ({ cards, className, cardClassName }: Props) => {
  return (
    <div className={`grid grid-cols-4 ${className}`}>
      {cards.map((card, i) => (
        <Card key={i} isRevealed={card !== "?"} card={card} cardClassName={cardClassName} />
      ))}
    </div>
  );
};

export default CardGridWrapper;
