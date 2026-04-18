import React from "react";
import Cart from "../shared/Cart.tsx";
import ScorePanel from "../shared/ScorePanel.tsx";

const OpponentPanel = () => {
  const carts = [5, 1, 2, "?", 2, "?", 5, -1, 2, 3, 1, 1];

  return (
    <div className="mx-auto flex w-full max-w-md items-center justify-between rounded-xl bg-zinc-950 p-5 shadow">
      <div className="grid w-1/2 grid-cols-4 gap-4">
        {carts.map(cart => (
          <Cart>{cart}</Cart>
        ))}
      </div>

      <ScorePanel />
    </div>
  );
};

export default OpponentPanel;
