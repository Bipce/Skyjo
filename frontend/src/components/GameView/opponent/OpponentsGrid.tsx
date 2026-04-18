import React, { type ReactNode } from "react";

interface Props {
  children: ReactNode;
}

const OpponentsGrid = ({ children }: Props) => {
  return <section className="flex items-center gap-10 overflow-hidden p-4">{children}</section>;
};

export default OpponentsGrid;
