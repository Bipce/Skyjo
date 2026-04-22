import type { CardData } from "../../../interfaces/CardData.ts";
import CardGridWrapper from "../shared/card/CardGridWrapper.tsx";
import ScorePanel from "../shared/score/ScorePanel.tsx";
import Separation from "../../ui/Separation.tsx";
import type { PlayerData } from "../../../interfaces/PlayerData.ts";

interface Props {
  player: PlayerData;
}

const OpponentPanel = ({ player }: Props) => {
  const cards: CardData[] = player.cards;

  return (
    <div className="panel-base relative mx-auto w-full max-w-md justify-between p-5">
      <Separation className="right-[38%]" />
      <CardGridWrapper cards={cards} className="w-1/2 gap-4" belongsTo="opponent" />
      <ScorePanel player={player} />
    </div>
  );
};

export default OpponentPanel;
