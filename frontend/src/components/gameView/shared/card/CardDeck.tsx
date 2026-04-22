import Card from "./Card.tsx";

const CardDeck = () => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Card card={{ number: 9, isRevealed: true }} belongsTo="deck" />
      <Card card={{ number: 5, isRevealed: false }} belongsTo="deck" />
    </section>
  );
};

export default CardDeck;
