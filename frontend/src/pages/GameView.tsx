import { useEffect, useState } from "react";
import OpponentsGrid from "../components/gameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/gameView/opponent/OpponentPanel.tsx";
import CardDeck from "../components/gameView/shared/card/CardDeck.tsx";
import PlayerPanel from "../components/gameView/player/PlayerPanel.tsx";
import type { PlayerData } from "../interfaces/PlayerData.ts";
import type { CardData } from "../interfaces/CardData.ts";

const GameView = () => {
  const [players, setPlayers] = useState<PlayerData[]>([]);
  const [player, setPlayer] = useState<PlayerData | null>(null);
  const [drawnCard, setDrawnCard] = useState<CardData | null>(null);
  const [discardedCard, setDiscardedCard] = useState<CardData | null>(null);

  useEffect(() => {
    window.startNetwork();

    window.addPlayer = (player: PlayerData) => {
      setPlayers(prev => [...prev, player]);
      if (player.isOwner) setPlayer(player);
    };

    window.removePlayer = (username: string) => {
      setPlayers(prev => prev.filter(player => player.username !== username));
    };

    window.updatePlayer = (username: string, playerData: PlayerData) => {
      setPlayers(prev => {
        const index = prev.findIndex(player => player.username === username);
        const data = [...prev];
        data[index] = playerData;
        return data;
      });

      if (playerData.isOwner) {
        setPlayer(playerData);
      }
    };

    window.initGame = (drawnCard: CardData, discardedCard: CardData) => {
      setDrawnCard(drawnCard);
      setDiscardedCard(discardedCard);
    };
  }, [player]);

  return (
    <main className="grid min-h-screen place-items-center py-10">
      <div className="grid w-full max-w-6xl gap-8">
        <OpponentsGrid>
          {players
            .filter(player => !player.isOwner)
            .map(player => {
              return <OpponentPanel key={player.username} player={player} />;
            })}
        </OpponentsGrid>

        {drawnCard && discardedCard && <CardDeck drawnCard={drawnCard} discardedCard={discardedCard} />}

        {player && <PlayerPanel player={player} />}
      </div>
    </main>
  );
};

export default GameView;
