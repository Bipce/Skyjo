import OpponentsGrid from "../components/gameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/gameView/opponent/OpponentPanel.tsx";
import CardDeck from "../components/gameView/shared/card/CardDeck.tsx";
import PlayerPanel from "../components/gameView/player/PlayerPanel.tsx";
import { useEffect, useState } from "react";
import type { PlayerData } from "../interfaces/PlayerData.ts";
import type { CardData } from "../interfaces/CardData.ts";

const GameView = () => {
  const [players, setPlayers] = useState<PlayerData[]>([]);
  const [player, setPlayer] = useState<PlayerData | null>(null);

  useEffect(() => {
    window.startNetwork();

    window.addPlayer = (player: PlayerData) => {
      setPlayers(prev => [...prev, player]);
      if (player.isOwner) setPlayer(player);
    };

    window.removePlayer = (username: string) => {
      setPlayers(prev => prev.filter(p => p.username !== username));
    };

    window.initGame = (drawCard: CardData, discardCard: CardData) => {
      // todo
    };
  }, []);

  return (
    <main className="grid min-h-screen place-items-center py-10">
      <div className="grid w-full max-w-6xl gap-8">
        <OpponentsGrid>
          {players
            .filter(x => !x.isOwner)
            .map(player => {
              return <OpponentPanel key={player.username} player={player} />;
            })}
        </OpponentsGrid>

        <CardDeck />

        {player && <PlayerPanel player={player} />}
      </div>
    </main>
  );
};

export default GameView;
