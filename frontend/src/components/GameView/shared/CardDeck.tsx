import Card from "./Card.tsx";

const CardDeck = () => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Card isRevealed card={9} className="text-5xl" />
      <Card isRevealed={false} card="?" className="text-2xl" />
    </section>
  );
};

export default CardDeck;
