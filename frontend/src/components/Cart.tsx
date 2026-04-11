interface Props {
  number: number;
  isYours: boolean;
}

const Cart = ({ number, isYours }: Props) => {
  return (
    <div className={`center border-round text-2xl font-bold ${isYours ? "h-28 w-16" : "h-16 w-10"}`}>{number}</div>
  );
};

export default Cart;
