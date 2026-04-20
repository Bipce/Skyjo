import ScorePanel from "../shared/ScorePanel.tsx";
import CardGridWrapper from "../shared/CardGridWrapper.tsx";
import type { ICard } from "../../interfaces/ICard.ts";

const PlayerPanel = () => {
  const cards: ICard[] = [
    { number: 0, isRevealed: true, belongTo: "player" },
    { number: 1, isRevealed: false, belongTo: "player" },
    { number: 2, isRevealed: false, belongTo: "player" },
    { number: 5, isRevealed: true, belongTo: "player" },
    { number: 5, isRevealed: false, belongTo: "player" },
    { number: 9, isRevealed: true, belongTo: "player" },
    { number: 12, isRevealed: false, belongTo: "player" },
    { number: -2, isRevealed: true, belongTo: "player" },
    { number: 4, isRevealed: true, belongTo: "player" },
    { number: 5, isRevealed: false, belongTo: "player" },
    { number: 5, isRevealed: true, belongTo: "player" },
    { number: 5, isRevealed: true, belongTo: "player" },
  ];

  return (
    <section className="flex h-full w-full justify-center">
      <div className="flex h-full w-2/3 items-center justify-around gap-10 overflow-hidden rounded-xl bg-zinc-950 p-8 shadow">
        <CardGridWrapper cards={cards} className="w-full gap-10" />
        <ScorePanel />
      </div>
    </section>
  );
};

export default PlayerPanel;
