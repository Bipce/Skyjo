import { type ReactNode } from "react";

interface Props {
  children: ReactNode;
}

const IconWrapper = ({ children }: Props) => {
  return <p className="flex items-center gap-2 text-lg">{children}</p>;
};

export default IconWrapper;
