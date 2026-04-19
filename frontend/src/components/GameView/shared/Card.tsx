interface Props {
  isRevealed: boolean;
  card: number | string;
}

const Card = ({ isRevealed, card }: Props) => {
  const getCardColor = (cart: number | string) => {
    const value = Number(cart);
    if (value < 0) return "custom-card-purple";
    if (value === 0) return "custom-card-blue";
    if (value <= 4) return "custom-card-green";
    if (value <= 8) return "custom-card-yellow";
    return "custom-card-red";
  };

  const cardColor = getCardColor(card);

  return (
    <button
      className={`center border-round aspect-2/3 max-h-28 w-full max-w-20 bg-size-[100%_100%] bg-no-repeat text-3xl font-bold ${
        isRevealed ? `${cardColor}` : "custom-card-no-reveal"
      }`}
    >
      {isRevealed ? card : ""}
    </button>
  );
};

export default Card;
