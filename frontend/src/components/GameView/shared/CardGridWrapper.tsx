import Card from "./Card.tsx";

interface Props {
  className?: string;
  cards: Array<number | string>;
}

const CardGridWrapper = ({ cards, className }: Props) => {
  return (
    <div className={`grid grid-cols-4 ${className}`}>
      {cards.map((card, i) => (
        <Card key={i} isRevealed={card !== "?"} card={card} />
      ))}
    </div>
  );
};

export default CardGridWrapper;
