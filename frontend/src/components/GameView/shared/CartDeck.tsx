import Cart from "./Cart.tsx";

const CartDeck = () => {
  return (
    <section className="flex items-center justify-center gap-10">
      <Cart>9</Cart>
      <Cart>?</Cart>
    </section>
  );
};

export default CartDeck;
