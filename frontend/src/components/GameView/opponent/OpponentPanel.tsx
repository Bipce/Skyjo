import type { CardData } from "../../interfaces/CardData.ts";
import CardGridWrapper from "../shared/CardGridWrapper.tsx";
import ScorePanel from "../shared/ScorePanel.tsx";
import Separation from "../shared/Separation.tsx";

const OpponentPanel = () => {
  const cards: CardData[] = [
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
    <div className="panel-base relative mx-auto w-full max-w-md justify-between p-5">
      <Separation className="right-[38%]" />
      <CardGridWrapper cards={cards} className="w-1/2 gap-4" />
      <ScorePanel />
    </div>
  );
};

export default OpponentPanel;
