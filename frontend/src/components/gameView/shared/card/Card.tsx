import clsx from "clsx";
import type { CardBelongToType, CardData } from "../../../../interfaces/CardData.ts";
import { useGameStore } from "../../../../store/gameStore.ts";
import { useShallow } from "zustand/react/shallow";
import { useDraggable, useDroppable } from "@dnd-kit/react";
import { useDragActive } from "../../../../hooks/useDragActive.ts";

interface Props {
  card: CardData;
  belongsTo: CardBelongToType;
  className?: string;
  isDraggable?: boolean;
  isDroppable?: boolean;
  isDiscarded?: boolean;
}

const Card = ({ card, belongsTo, className, isDiscarded, isDraggable = false, isDroppable = false }: Props) => {
  const { number, isRevealed, isSelected } = card;
  const { selectCard, isCurrentPlayer, hasGameStarted, drawnCard } = useGameStore(
    useShallow(s => ({
      selectCard: s.selectCard,
      isCurrentPlayer: s.player?.isCurrentPlayer,
      hasGameStarted: s.players.some(player => player.isCurrentPlayer === true),
      drawnCard: s.drawnCard,
    })),
  );

  const isDragActive = useDragActive();

  const { ref: refDrag, isDragSource } = useDraggable({ id: card.id, disabled: !isDraggable || !isCurrentPlayer });
  const { ref: refDrop, isDropTarget } = useDroppable({ id: card.id, disabled: !isDroppable || !isCurrentPlayer });

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

  const handleSelected = (cardId: number) => {
    if (belongsTo === "player") selectCard(cardId);
    if (belongsTo === "deck" && !isRevealed) selectCard(cardId);
  };

  const isDrawnCardRevealed = drawnCard?.isRevealed ?? false;
  const canHover =
    belongsTo !== "opponent" && (!hasGameStarted || isCurrentPlayer) && !(isDiscarded && isDrawnCardRevealed);

  const buttonClass = clsx(
    "center button-card-base card-number",
    className,
    cardColor,
    cardSize,
    isSelected ? "border border-rose-600 shadow-md shadow-rose-600 hover:ring-rose-600" : "border border-zinc-500",
    isDroppable && isDropTarget && isDragActive && "ring-2 ring-amber-300/80",
    canHover && "hover:ring-2 hover:ring-amber-300/80",
  );

  const button = (
    <button ref={isDroppable ? refDrop : undefined} onClick={() => handleSelected(card.id)} className={buttonClass}>
      {isRevealed ? number : <span className="-rotate-45 tracking-widest text-zinc-100">SKYJO</span>}
    </button>
  );

  if (!isDraggable) return button;

  return (
    <div ref={refDrag} className={`w-full max-w-20 ${isDragSource ? "opacity-0" : "opacity-100"} `}>
      {button}
    </div>
  );
};

export default Card;
