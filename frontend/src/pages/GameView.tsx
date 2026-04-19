import OpponentsGrid from "../components/GameView/opponent/OpponentsGrid.tsx";
import OpponentPanel from "../components/GameView/opponent/OpponentPanel.tsx";
import PlayerPanel from "../components/GameView/player/PlayerPanel.tsx";
import CardDeck from "../components/GameView/shared/CardDeck.tsx";

const GameView = () => {
  return (
    <main className="grid min-h-screen place-items-center py-10">
      <div className="grid w-full max-w-6xl gap-8">
        <OpponentsGrid>
          <OpponentPanel />
          <OpponentPanel />
          {/*<OpponentPanel />*/}
        </OpponentsGrid>

        <CardDeck />

        <PlayerPanel />
      </div>
    </main>
  );
};

export default GameView;
