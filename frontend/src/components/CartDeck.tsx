import { type ReactNode } from "react";

interface Props {
  children: ReactNode;
}

const CartDeck = ({ children }: Props) => {
  return <div className="border-round center h-32 w-20 text-2xl font-bold">{children}</div>;
};

export default CartDeck;
