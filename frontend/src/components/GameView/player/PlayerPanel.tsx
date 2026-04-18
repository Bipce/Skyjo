import React from "react";
import Cart from "../shared/Cart.tsx";
import ScorePanel from "../shared/ScorePanel.tsx";

const PlayerPanel = () => {
  const carts = [5, 1, 2, "?", 2, "?", 5, -1, 2, 3, 1, 1];

  return (
    <section className="flex h-full w-full justify-center">
      <div className="flex h-full w-2/3 items-center justify-around gap-10 overflow-hidden rounded-xl bg-zinc-950 p-8 shadow">
        <div className="grid w-full min-w-0 grid-cols-4 content-center gap-10">
          {carts.map(cart => (
            <Cart>{cart}</Cart>
          ))}
        </div>

        <ScorePanel />
      </div>
    </section>
  );
};

export default PlayerPanel;
