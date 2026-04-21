import OpponentsGrid from "../components/gameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/gameView/opponent/OpponentPanel.tsx";
import CardDeck from "../components/gameView/shared/card/CardDeck.tsx";
import PlayerPanel from "../components/gameView/player/PlayerPanel.tsx";
import { useEffect, useState } from "react";

const GameView = () => {
  const [username, setUsername] = useState("");

  useEffect(() => {
    window.startNetwork();
    window.setUsername = setUsername;
  }, [])

  return (
    <main className="grid min-h-screen place-items-center py-10">
      <div className="grid w-full max-w-6xl gap-8">
        <OpponentsGrid>
          {/*<OpponentPanel />*/}
          {/*<OpponentPanel />*/}
          {/*<OpponentPanel />*/}
        </OpponentsGrid>

        <CardDeck />

        <PlayerPanel username={username} />
      </div>
    </main>
  );
};

export default GameView;
