interface Props {
  isRevealed: boolean;
  card: number | string;
  className?: string;
  cardClassName?: string;
}

const Card = ({ isRevealed, card, className, cardClassName }: Props) => {
  const getCardColor = (cart: number | string) => {
    const value = Number(cart);
    if (value < 0) return "custom-card-dark-blue";
    if (value === 0) return "custom-card-blue";
    if (value <= 4) return "custom-card-green";
    if (value <= 8) return "custom-card-yellow";
    return "custom-card-red";
  };

  const cardColor = getCardColor(card);

  return (
    <button
      className={`center border-round t aspect-2/3 max-h-28 w-full max-w-20 bg-size-[100%_100%] bg-no-repeat font-bold ${
        isRevealed ? `${cardColor}` : `custom-card-no-reveal ${cardClassName}`
      } ${className} card-number card-pattern text-zinc-950`}
    >
      {isRevealed ? card : <span className="-rotate-45 tracking-widest text-zinc-100">SKYJO</span>}
    </button>
  );
};

export default Card;
