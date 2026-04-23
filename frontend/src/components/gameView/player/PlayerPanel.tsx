import ScorePanel from "../shared/score/ScorePanel.tsx";
import Separation from "../../ui/Separation.tsx";
import CardGridWrapper from "../shared/card/CardGridWrapper.tsx";
import type { PlayerData } from "../../../interfaces/PlayerData.ts";
import type { CardData } from "../../../interfaces/CardData.ts";

interface Props {
  player: PlayerData;
}

const PlayerPanel = ({ player }: Props) => {
  const { cards } = player;
  const playerCards: CardData[] = cards;

  const handleSelectedForInitiateGame = (cardId: number) => {
    window.selectCard(player.id, cardId);
  };

  return (
    <section className="relative flex h-full w-full justify-center">
      <Separation className="right-1/3" />
      <div className="panel-base h-full w-2/3 justify-around gap-10 p-8">
        <CardGridWrapper
          cards={playerCards}
          className="w-full gap-10"
          belongsTo="player"
          onSelectedForRevealCard={handleSelectedForInitiateGame}
        />
        <ScorePanel player={player} />
      </div>
    </section>
  );
};

export default PlayerPanel;
