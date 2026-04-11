import React from "react";
import Cart from "./Cart.tsx";

interface Props {
  isYours: boolean;
}

const PlayerPanel = ({ isYours }: Props) => {
  return (
    <div className="center">
      <div className={`grid grid-cols-4 ${isYours ? "gap-10" : "gap-5"} `}>
        <Cart number={-2} isYours={isYours} />
        <Cart number={-1} isYours={isYours} />
        <Cart number={0} isYours={isYours} />
        <Cart number={1} isYours={isYours} />
        <Cart number={2} isYours={isYours} />
        <Cart number={3} isYours={isYours} />
        <Cart number={2} isYours={isYours} />
        <Cart number={2} isYours={isYours} />
        <Cart number={2} isYours={isYours} />
        <Cart number={2} isYours={isYours} />
        <Cart number={2} isYours={isYours} />
        <Cart number={2} isYours={isYours} />
      </div>

      <div className="border-round ml-auto flex w-48 flex-col items-center justify-between gap-5 p-5">
        {!isYours && <p>Pseudo</p>}
        <p>Score partie : 12</p>
        <p>Score global : 32</p>
      </div>
    </div>
  );
};

export default PlayerPanel;
