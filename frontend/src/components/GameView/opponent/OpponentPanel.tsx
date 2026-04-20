import type { ICard } from "../../interfaces/ICard.ts";
import CardGridWrapper from "../shared/CardGridWrapper.tsx";
import ScorePanel from "../shared/ScorePanel.tsx";
import card from "../shared/Card.tsx";

const OpponentPanel = () => {
  const cards: ICard[] = [
    { number: 0, isRevealed: true, belongTo: "opponent" },
    { number: 1, isRevealed: false, belongTo: "opponent" },
    { number: 2, isRevealed: false, belongTo: "opponent" },
    { number: 5, isRevealed: true, belongTo: "opponent" },
    { number: 5, isRevealed: false, belongTo: "opponent" },
    { number: 9, isRevealed: true, belongTo: "opponent" },
    { number: 12, isRevealed: false, belongTo: "opponent" },
    { number: 5, isRevealed: false, belongTo: "opponent" },
    { number: 4, isRevealed: true, belongTo: "opponent" },
    { number: 5, isRevealed: false, belongTo: "opponent" },
    { number: 5, isRevealed: true, belongTo: "opponent" },
    { number: 5, isRevealed: true, belongTo: "opponent" },
  ];

  return (
    <div className="mx-auto flex w-full max-w-md items-center justify-between rounded-xl bg-zinc-950 p-5 shadow">
      <CardGridWrapper cards={cards} className="w-1/2 gap-4" />
      <ScorePanel />
    </div>
  );
};

export default OpponentPanel;
