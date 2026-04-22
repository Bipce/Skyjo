import ScorePanel from "../shared/score/ScorePanel.tsx";
import CardGridWrapper from "../shared/card/CardGridWrapper.tsx";
import type { CardData } from "../../../interfaces/CardData.ts";
import Separation from "../../ui/Separation.tsx";
import type { PlayerData } from "../../../interfaces/PlayerData.ts";

interface Props {
  player: PlayerData;
}

const PlayerPanel = ({ player }: Props) => {
  const cards: CardData[] = player.cards;

  return (
    <section className="relative flex h-full w-full justify-center">
      <Separation className="right-1/3" />
      <div className="panel-base h-full w-2/3 justify-around gap-10 p-8">
        <CardGridWrapper cards={cards} className="w-full gap-10" belongsTo="player" />
        <ScorePanel player={player} />
      </div>
    </section>
  );
};

export default PlayerPanel;
