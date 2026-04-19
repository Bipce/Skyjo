import Card from "./Card.tsx";

const CardDeck = () => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Card isRevealed card={9} />
      <Card isRevealed={false} card="?" />
    </section>
  );
};

export default CardDeck;
