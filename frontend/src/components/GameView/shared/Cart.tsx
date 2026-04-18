import type { ReactNode } from "react";

interface Props {
  children: ReactNode;
}

const Cart = ({ children }: Props) => {
  return <div className="center border-round aspect-2/3 max-h-28 w-full max-w-20 text-2xl">{children}</div>;
};

export default Cart;
