import Card from "./Card.tsx";

const CardDeck = () => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Card card={{ number: 9, isRevealed: true }} className="text-5xl" />
      <Card card={{ number: 5, isRevealed: false }} className="text-xl" />
    </section>
  );
};

export default CardDeck;
