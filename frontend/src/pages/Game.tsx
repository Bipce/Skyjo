import PlayerPanel from "../components/PlayerPanel.tsx";
import Wrapper from "../components/Wrapper.tsx";
import CartDeck from "../components/CartDeck.tsx";

const Game = () => {
  return (
    <main className="center h-full min-h-screen w-full flex-col gap-10 p-5">
      <div className="flex min-w-full gap-10">
        <Wrapper isYours={false}>
          <PlayerPanel isYours={false} />
        </Wrapper>
        <div className="flex flex-col gap-5">
          <CartDeck>?</CartDeck>
          <CartDeck>9</CartDeck>
        </div>
      </div>

      <Wrapper isYours={true}>
        <PlayerPanel isYours={true} />
      </Wrapper>
    </main>
  );
};

export default Game;
