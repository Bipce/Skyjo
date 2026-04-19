import type { ICard } from "../../interfaces/ICard.ts";
import CardGridWrapper from "../shared/CardGridWrapper.tsx";
import ScorePanel from "../shared/ScorePanel.tsx";

const OpponentPanel = () => {
  const cards: ICard[] = [
    { number: 0, isRevealed: true },
    { number: 1, isRevealed: false },
    { number: 2, isRevealed: false },
    { number: 5, isRevealed: true },
    { number: 5, isRevealed: false },
    { number: 9, isRevealed: true },
    { number: 12, isRevealed: false },
    { number: 5, isRevealed: false },
    { number: 4, isRevealed: true },
    { number: 5, isRevealed: false },
    { number: 5, isRevealed: true },
    { number: 5, isRevealed: true },
  ];

  return (
    <div className="mx-auto flex w-full max-w-md items-center justify-between rounded-xl bg-zinc-950 p-5 shadow">
      <CardGridWrapper cards={cards} className="w-1/2 gap-4 text-3xl" cardClassName="text-[9px]" />
      <ScorePanel />
    </div>
  );
};

export default OpponentPanel;
