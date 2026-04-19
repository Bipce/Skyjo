import type { ICard } from "../../interfaces/ICard.ts";

interface Props {
  card: ICard;
  className?: string;
  cardClassName?: string;
}

const Card = ({ card, className, cardClassName }: Props) => {
  const getCardColor = (card: ICard) => {
    const { number } = card;

    if (number < 0) return "custom-card-dark-blue";
    if (number === 0) return "custom-card-blue";
    if (number <= 4) return "custom-card-green";
    if (number <= 8) return "custom-card-yellow";
    if (number > 8) return "custom-card-red";

    return "custom-card-no-reveal";
  };

  const cardColor = getCardColor(card);

  return (
    <button
      className={`center border-round t aspect-2/3 max-h-28 w-full max-w-20 bg-size-[100%_100%] bg-no-repeat font-bold ${
        card.isRevealed && cardColor
      } ${cardClassName} ${className} card-number card-pattern text-zinc-950`}
    >
      {card.isRevealed ? card.number : <span className="-rotate-45 tracking-widest text-zinc-100">SKYJO</span>}
    </button>
  );
};

export default Card;
