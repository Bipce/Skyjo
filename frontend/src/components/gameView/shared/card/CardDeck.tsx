import Card from "./Card.tsx";

const CardDeck = () => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Card card={{ number: 9, isRevealed: true, belongTo: "deck" }} />
      <Card card={{ number: 5, isRevealed: false, belongTo: "deck" }} />
    </section>
  );
};

export default CardDeck;
