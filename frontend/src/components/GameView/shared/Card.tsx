import type { CardData } from "../../interfaces/CardData.ts";

interface Props {
  card: CardData;
  className?: string;
}

const Card = ({ card, className }: Props) => {
  const { number, isRevealed, belongTo } = card;
  const getCardColor = (): string => {
    if (isRevealed) {
      if (number < 0) return "custom-card-dark-blue";
      if (number === 0) return "custom-card-blue";
      if (number <= 4) return "custom-card-green";
      if (number <= 8) return "custom-card-yellow";
      if (number > 8) return "custom-card-red";
    }
    return "custom-card-no-reveal";
  };
  const cardColor = getCardColor();

  const getCardSize = (): string => {
    switch (belongTo) {
      case "opponent":
        return isRevealed ? "text-3xl" : "text-[9px]";
      case "player":
      case "deck":
        return isRevealed ? "text-5xl" : "text-xl";
      default:
        return "";
    }
  };
  const cardSize = getCardSize();

  return (
    <button
      className={`center border-round t } aspect-2/3 max-h-28 w-full max-w-20 bg-size-[100%_100%] bg-no-repeat font-bold ${
        className
      } ${cardColor} ${cardSize} card-number card-pattern text-zinc-950`}
    >
      {card.isRevealed ? card.number : <span className="-rotate-45 tracking-widest text-zinc-100">SKYJO</span>}
    </button>
  );
};

export default Card;
