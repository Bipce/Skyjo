import type { CardBelongToType, CardData } from "../../../../interfaces/CardData.ts";
import { useGameStore } from "../../../../store/gameStore.ts";

interface Props {
  card: CardData;
  belongsTo: CardBelongToType;
  className?: string;
}

const Card = ({ card, belongsTo, className }: Props) => {
  const { number, isRevealed, isSelected } = card;
  const selectCard = useGameStore(s => s.selectCard);

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
    switch (belongsTo) {
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

  const handleSelectedForInitiateGame = (cardId: number) => {
    if (belongsTo === "player") selectCard(cardId);
  };

  return (
    <button
      onClick={() => handleSelectedForInitiateGame(card.id)}
      className={`center aspect-2/3 max-h-28 w-full max-w-20 rounded-xl bg-size-[100%_100%] bg-no-repeat font-bold ${
        isSelected ? "border-2 border-rose-600 shadow-md shadow-rose-600" : "border border-zinc-500"
      } ${className} ${cardColor} ${cardSize} card-number card-pattern text-zinc-950`}
    >
      {isRevealed ? number : <span className="-rotate-45 tracking-widest text-zinc-100">SKYJO</span>}
    </button>
  );
};

export default Card;
