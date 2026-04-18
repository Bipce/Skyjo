import OpponentsGrid from "../components/GameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/GameView/opponent/OpponentPanel.tsx";
import CartDeck from "../components/GameView/CartDeck.tsx";
import PlayerPanel from "../components/GameView/player/PlayerPanel.tsx";

const GameView = () => {
  return (
    <main className="grid h-screen grid-rows-[1fr_auto_1.5fr]">
      <OpponentsGrid>
        <OpponentPanel />
        <OpponentPanel />
      </OpponentsGrid>

      <CartDeck />

      <PlayerPanel />
    </main>
  );
};

export default GameView;
