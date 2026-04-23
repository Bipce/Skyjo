import { useEffect } from "react";
import { useShallow } from "zustand/react/shallow";
import OpponentsGrid from "../components/gameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/gameView/opponent/OpponentPanel.tsx";
import CardDeck from "../components/gameView/shared/card/CardDeck.tsx";
import PlayerPanel from "../components/gameView/player/PlayerPanel.tsx";
import { useGameStore } from "../store/gameStore.ts";

const GameView = () => {
  const { bindWindowCallbacks, players, player, drawnCard, discardedCard } = useGameStore(
    useShallow(s => ({
      bindWindowCallbacks: s.bindWindowCallbacks,
      players: s.players,
      player: s.player,
      drawnCard: s.drawnCard,
      discardedCard: s.discardedCard,
    })),
  );

  useEffect(() => {
    bindWindowCallbacks();
  }, [bindWindowCallbacks]);

  return (
    <main className="grid min-h-screen place-items-center py-10">
      <div className="grid w-full max-w-6xl gap-8">
        <OpponentsGrid>
          {players
            .filter(player => !player.isOwner)
            .map(player => {
              return <OpponentPanel key={player.id} player={player} />;
            })}
        </OpponentsGrid>

        {drawnCard && discardedCard && <CardDeck drawnCard={drawnCard} discardedCard={discardedCard} />}

        {player && <PlayerPanel player={player} />}
      </div>
    </main>
  );
};

export default GameView;
