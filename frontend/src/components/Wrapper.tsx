import { type ReactNode } from "react";

interface Props {
  children: ReactNode;
  isYours: boolean;
}

const Wrapper = ({ children, isYours }: Props) => {
  return (
    <div
      className={`size-full rounded-lg bg-zinc-950 p-8 shadow ${isYours ? "max-h-2/3 max-w-2/3" : "max-h-1/12 max-w-1/2"}`}
    >
      {children}
    </div>
  );
};

export default Wrapper;
