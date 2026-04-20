import ScorePanel from "../shared/ScorePanel.tsx";
import CardGridWrapper from "../shared/CardGridWrapper.tsx";
import type { CardData } from "../../interfaces/CardData.ts";
import Separation from "../shared/Separation.tsx";

const PlayerPanel = () => {
  const cards: CardData[] = [
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
    <section className="relative flex h-full w-full justify-center">
      <Separation className="right-1/3" />
      <div className="panel-base h-full w-2/3 justify-around gap-10 p-8">
        <CardGridWrapper cards={cards} className="w-full gap-10" />
        <ScorePanel />
      </div>
    </section>
  );
};

export default PlayerPanel;
