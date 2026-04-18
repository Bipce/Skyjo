import OpponentsGrid from "../components/GameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/GameView/opponent/OpponentPanel.tsx";
import CartDeck from "../components/GameView/CartDeck.tsx";
import PlayerPanel from "../components/GameView/player/PlayerPanel.tsx";

const GameView = () => {
  return (
    <main className="grid h-screen place-items-center px-6">
      <div className="grid w-full max-w-5xl gap-6">
        <OpponentsGrid>
          <OpponentPanel />
          <OpponentPanel />
        </OpponentsGrid>

        <CartDeck />

        <PlayerPanel />
      </div>
    </main>
  );
};

export default GameView;
